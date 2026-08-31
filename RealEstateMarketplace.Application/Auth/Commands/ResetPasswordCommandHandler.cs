using MediatR;
using Microsoft.AspNetCore.Identity;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Auth.Commands;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<Unit, AuthError>>
{
    private readonly UserManager<User> _userManager;

    public ResetPasswordCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<Unit, AuthError>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            // Same generic message as an invalid/expired token - don't reveal whether the email exists.
            return Result.Failure<Unit, AuthError>(AuthError.InvalidResetToken());
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

        if (!result.Succeeded)
        {
            var isTokenError = result.Errors.Any(e => e.Code.Contains("Token", StringComparison.OrdinalIgnoreCase));

            if (isTokenError)
            {
                return Result.Failure<Unit, AuthError>(AuthError.InvalidResetToken());
            }

            var message = string.Join(" ", result.Errors.Select(e => e.Description));
            return Result.Failure<Unit, AuthError>(AuthError.PasswordResetFailed(message));
        }

        return Result.Success<Unit, AuthError>(Unit.Value);
    }
}