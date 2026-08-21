using MediatR;
using Microsoft.AspNetCore.Identity;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Auth.Commands;

public sealed class LoginCommandHandler
    : IRequestHandler<
        LoginCommand,
        Result<AuthResponseDto, AuthError>>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        UserManager<User> userManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponseDto, AuthError>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userManager.FindByEmailAsync(
                request.Email);

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

        var roles =
            await _userManager.GetRolesAsync(user);

        var accessToken =
            _tokenService.GenerateAccessToken(
                user,
                roles);

        return Result.Success<AuthResponseDto, AuthError>(
            new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = user.Role.ToString(),
                Token = accessToken
            });
    }
}