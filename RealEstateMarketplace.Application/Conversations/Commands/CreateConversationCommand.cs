using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Conversations.Commands;

public sealed class CreateConversationCommand : IRequest<ConversationDto>
{
    public int PropertyId { get; set; }
    public int BuyerId { get; set; }
    public int OwnerId { get; set; }
}
