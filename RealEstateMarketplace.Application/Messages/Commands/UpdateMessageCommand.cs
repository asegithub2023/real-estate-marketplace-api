using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Messages.Commands;

public sealed class UpdateMessageCommand : IRequest<MessageDto?>
{
    public int Id { get; set; }
    public string? Content { get; set; }
}
