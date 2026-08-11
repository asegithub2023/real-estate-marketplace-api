using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Conversations.Commands;

public sealed class UpdateConversationCommand : IRequest<Result<Conversation, ConversationError>>
{
    public int Id { get; set; }
    public int? PropertyId { get; set; }
    public int? BuyerId { get; set; }
    public int? OwnerId { get; set; }
}
