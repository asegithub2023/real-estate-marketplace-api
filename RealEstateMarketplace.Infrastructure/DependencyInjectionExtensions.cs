using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Application.Validators;
using RealEstateMarketplace.Infrastructure.Repositories;
using RealEstateMarketplace.Infrastructure.Services;

namespace RealEstateMarketplace.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
        services.AddScoped<IPropertyFeatureRepository, PropertyFeatureRepository>();
        services.AddScoped<IFavoriteRepository, FavoriteRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<ICachedPropertyService, CachedPropertyService>();
        services.AddScoped<IPropertyImageService, PropertyImageService>();
        services.AddScoped<IPropertyFeatureService, PropertyFeatureService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }
}
