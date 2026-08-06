using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Conversations.Commands;

public sealed class UpdateConversationCommand : IRequest<Result<ConversationDto, ConversationError>>
{
    public int Id { get; set; }
    public int? PropertyId { get; set; }
    public int? BuyerId { get; set; }
    public int? OwnerId { get; set; }
}
