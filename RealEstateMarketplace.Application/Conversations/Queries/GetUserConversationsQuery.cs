using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Conversations.Queries;

public sealed class GetUserConversationsQuery : IRequest<IReadOnlyList<ConversationDto>>
{
    public int UserId { get; set; }
}
