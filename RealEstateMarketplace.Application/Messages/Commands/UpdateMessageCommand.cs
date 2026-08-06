using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Messages.Commands;

public sealed class UpdateMessageCommand : IRequest<Result<MessageDto, MessageError>>
{
    public int Id { get; set; }
    public string? Content { get; set; }
}
