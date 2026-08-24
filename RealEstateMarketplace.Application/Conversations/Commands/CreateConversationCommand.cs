using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Conversations.Commands;

public sealed class CreateConversationCommand : IRequest<Result<Conversation, ConversationError>>
{
    public int PropertyId { get; set; }

    // Set by the controller from the authenticated user's JWT claim - never trust
    // a client-supplied BuyerId.
    public int BuyerId { get; set; }
}