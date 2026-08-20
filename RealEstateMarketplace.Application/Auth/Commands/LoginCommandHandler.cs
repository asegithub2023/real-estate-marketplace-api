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

namespace RealEstateMarketplace.Application.Auth.Commands;

public sealed class LoginCommandHandler
    : IRequestHandler<
        LoginCommand,
        Result<AuthResponseDto, AuthError>>
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(
        UserManager<User> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<Result<AuthResponseDto, AuthError>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result.Failure<AuthResponseDto, AuthError>(
                AuthError.InvalidCredentials());
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return Result.Failure<AuthResponseDto, AuthError>(
                new AuthError(
                    "account_locked",
                    "Account locked due to multiple failed login attempts. Try again in 15 minutes."));
        }

        var validPassword =
            await _userManager.CheckPasswordAsync(
                user,
                request.Password);

        if (!validPassword)
        {
            await _userManager.AccessFailedAsync(user);

            return Result.Failure<AuthResponseDto, AuthError>(
                AuthError.InvalidCredentials());
        }

        await _userManager.ResetAccessFailedCountAsync(user);

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