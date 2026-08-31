using MediatR;

namespace RealEstateMarketplace.Application.Auth.Commands;

// Always succeeds regardless of whether the email exists, to avoid leaking
// which emails are registered (user enumeration).
public sealed class ForgotPasswordCommand : IRequest<Unit>
{
    public required string Email { get; set; }
}