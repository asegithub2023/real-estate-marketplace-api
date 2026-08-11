using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Conversations.Queries;

public sealed class GetUserConversationsQuery : IRequest<IReadOnlyList<Conversation>>
{
    public int UserId { get; set; }
}
