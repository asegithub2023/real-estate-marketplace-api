using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Messages.Commands;

public sealed class UpdateMessageCommand : IRequest<Result<Message, MessageError>>
{
    public int Id { get; set; }
    public string? Content { get; set; }
}
