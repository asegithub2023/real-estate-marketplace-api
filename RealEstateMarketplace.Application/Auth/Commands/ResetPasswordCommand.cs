using MediatR;
using RealEstateMarketplace.Application.Common;

namespace RealEstateMarketplace.Application.Auth.Commands;

public sealed class ResetPasswordCommand : IRequest<Result<Unit, AuthError>>
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
}