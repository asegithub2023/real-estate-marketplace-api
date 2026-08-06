using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Auth.Commands;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        var token = GenerateToken(user);
        return new AuthResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            Token = token
        };
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(storedHash))
        {
            return false;
        }

        if (storedHash.StartsWith("pbkdf2$", StringComparison.Ordinal))
        {
            var parts = storedHash.Split('$', 4);
            if (parts.Length != 4)
            {
                return false;
            }

            var iterations = int.Parse(parts[1]);
            var salt = Convert.FromBase64String(parts[2]);
            var hash = Convert.FromBase64String(parts[3]);

            var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, hash.Length);
            return CryptographicOperations.FixedTimeEquals(computedHash, hash);
        }

        return false;
    }

    private string GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = jwtSettings["Key"] ?? "default-development-secret-key-change-me";
        var issuer = jwtSettings["Issuer"] ?? "RealEstateMarketplace.Api";
        var audience = jwtSettings["Audience"] ?? "RealEstateMarketplace.Client";
        var expiresInMinutes = int.TryParse(jwtSettings["ExpiresInMinutes"], out var minutes) ? minutes : 60;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
