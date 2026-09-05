using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Domain.Entities;
using RealEstateMarketplace.Domain.Enums;

namespace RealEstateMarketplace.Application.Auth.Commands;

public sealed class RegisterCommandHandler
    : IRequestHandler<
        RegisterCommand,
        Result<AuthResponseDto, AuthError>>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IConfiguration _configuration;

    public RegisterCommandHandler(
        UserManager<User> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<Result<AuthResponseDto, AuthError>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser =
            await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return Result.Failure<AuthResponseDto, AuthError>(
                AuthError.EmailAlreadyExists(request.Email));
        }

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Role = Role.Seeker
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            var message = string.Join(
                " ",
                result.Errors.Select(e => e.Description));

            return Result.Failure<AuthResponseDto, AuthError>(
                new AuthError(
                    "password_policy",
                    message));
        }

        const string roleName = nameof(Role.Seeker);

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(
                new IdentityRole<int>(roleName));
        }

        await _userManager.AddToRoleAsync(
            user,
            roleName);

        var token = GenerateToken(user);

        return Result.Success<AuthResponseDto, AuthError>(
            new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = user.Role.ToString(),
                Token = token
            });
    }

    private string GenerateToken(User user)
    {
        var jwtSettings =
            _configuration.GetSection("JwtSettings");

        var key = jwtSettings["Key"]
            ?? "default-development-secret-key-change-me";

        var issuer = jwtSettings["Issuer"]
            ?? "RealEstateMarketplace.Api";

        var audience = jwtSettings["Audience"]
            ?? "RealEstateMarketplace.Client";

        var expiresInMinutes =
            int.TryParse(
                jwtSettings["ExpiresInMinutes"],
                out var minutes)
                ? minutes
                : 60;

        var securityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

        var credentials =
            new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                user.Email ?? string.Empty),

            new(
                ClaimTypes.Name,
                user.FullName),

            new(
                ClaimTypes.Role,
                user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                expiresInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}
