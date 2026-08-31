using Scalar.AspNetCore;
using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using RealEstateMarketplace.Api.ExceptionHandlers;
using RealEstateMarketplace.Api.RateLimiting;
using RealEstateMarketplace.Api.Utilities;
using RealEstateMarketplace.Api.Filters;
using RealEstateMarketplace.Api.Middlewares;
using RealEstateMarketplace.Api.Security;
using RealEstateMarketplace.Application.Reviews.Commands;
using RealEstateMarketplace.Infrastructure;
using RealEstateMarketplace.Infrastructure.Persistence;
using RealEstateMarketplace.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using RealEstateMarketplace.Domain.Entities;
using RealEstateMarketplace.Application.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;
using RealEstateMarketplace.Api.Hubs;

//using Polly;
//using Polly.CircuitBreaker;
//using Polly.Retry;
//using Polly.Timeout;

//using Microsoft.AspNetCore.Diagnostics.HealthChecks;
//using Microsoft.Extensions.Diagnostics.HealthChecks;
//using HealthChecks.NpgSql;
//using OpenTelemetry.Instrumentation.Runtime;
//using OpenTelemetry.Metrics;
//using OpenTelemetry.Resources;
//using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services
    .AddIdentityCore<User>(options =>
    {
        // Password policy
        options.Password.RequiredLength = 12;
        options.Password.RequireUppercase = true;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = true;

        // Lockout policy
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("postgresql");

builder.Services.AddApplicationServices();
builder.Services.AddSignalR();
builder.Services.AddMediatR(typeof(RealEstateMarketplace.Application.Reviews.Commands.CreateReviewCommand));
builder.Services.AddAutoMapper(
    typeof(RealEstateMarketplace.Api.Mapping.FavoritesProfile),
    typeof(RealEstateMarketplace.Api.Mapping.ConversationMessageProfile),
    typeof(RealEstateMarketplace.Api.Mapping.PropertiesProfile));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwt["Key"]
                            ?? throw new InvalidOperationException(
                                "JWT key is not configured.")))
            };

            options.Events = new JwtBearerEvents
{
    OnAuthenticationFailed = context =>
    {
        Console.WriteLine("🔥 JWT AUTH FAILED:");
        Console.WriteLine(context.Exception.Message);
        return Task.CompletedTask;
    },
    OnTokenValidated = context =>
    {
        Console.WriteLine("✅ JWT TOKEN VALIDATED");
        return Task.CompletedTask;
    },
    // SignalR's browser client can't set an Authorization header on a WebSocket
    // upgrade request, so it sends the token as ?access_token=... instead. Only
    // honor that for the hub path - every other endpoint still requires a real
    // Authorization header.
    OnMessageReceived = context =>
    {
        var accessToken = context.Request.Query["access_token"];
        var path = context.HttpContext.Request.Path;

        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
        {
            context.Token = accessToken;
        }

        return Task.CompletedTask;
    }
};
    });



builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.AdminOnly, policy => policy.RequireRole("Admin"));
    options.AddPolicy(Policies.OwnerOrAdmin, policy => policy.RequireRole("Owner", "Admin"));
    options.AddPolicy(Policies.SeekerOrOwnerOrAdmin, policy => policy.RequireRole("Seeker", "Owner", "Admin"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:4200"];
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});




builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var (partitionKey, tier) = ApiKeyResolver.Resolve(httpContext);

        return tier switch
        {
            ApiKeyTier.Paid => RateLimitPartition.GetTokenBucketLimiter(
                partitionKey: $"paid:{partitionKey}",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 200,
                    TokensPerPeriod = 100,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }),
            ApiKeyTier.Free => RateLimitPartition.GetTokenBucketLimiter(
                partitionKey: $"free:{partitionKey}",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 30,
                    TokensPerPeriod = 10,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }),
            _ => RateLimitPartition.GetTokenBucketLimiter(
                partitionKey: $"anon:{partitionKey}",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 10,
                    TokensPerPeriod = 5,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                    QueueLimit = 0,
                    AutoReplenishment = true
                })
        };
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, ct) =>
    {
        var retryAfter = "10";
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var ts))
            retryAfter = ((int)ts.TotalSeconds).ToString();

        context.HttpContext.Response.Headers.RetryAfter = retryAfter;
        context.HttpContext.Response.ContentType = "application/problem+json";

        await context.HttpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = "Rate limit exceeded",
            Detail = $"Too many requests. Retry after {retryAfter} seconds.",
            Status = StatusCodes.Status429TooManyRequests,
            Type = "https://tms.local/errors/rate_limit_exceeded"//
        }, ct);
    };

    options.AddConcurrencyLimiter("transcripts", opt =>
    {
        opt.PermitLimit = 5;
        opt.QueueLimit = 20;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(10),
        LocalCacheExpiration = TimeSpan.FromMinutes(2)
    };
});



builder.Services.AddScoped<IHateoasHelper, HateoasHelper>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<ITokenService, TokenService>();




var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/{documentName}.json");
    app.MapScalarApiReference("/scalar");
}


app.UseMiddleware<RequestLoggingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseExceptionHandler();
app.UseRouting();
app.UseCors("AllowAngularApp");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");
app.MapHealthChecks("/health");

app.Run();