using MediatR;

namespace RealEstateMarketplace.Application.Auth.Commands;

public sealed class ForgotPasswordCommand : IRequest<Unit>
{
    public required string Email { get; set; }
}
