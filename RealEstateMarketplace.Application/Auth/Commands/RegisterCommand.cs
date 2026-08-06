using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Auth.Commands;

public sealed class RegisterCommand : IRequest<Result<AuthResponseDto, AuthError>>
{
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string PhoneNumber { get; set; }
}
