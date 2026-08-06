using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Conversations.Commands;

public sealed class CreateConversationCommand : IRequest<Result<ConversationDto, ConversationError>>
{
    public int PropertyId { get; set; }
    public int BuyerId { get; set; }
    public int OwnerId { get; set; }
}
