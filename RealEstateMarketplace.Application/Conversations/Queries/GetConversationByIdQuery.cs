using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Conversations.Queries;

public sealed class GetConversationByIdQuery : IRequest<Conversation?>
{
    public int Id { get; set; }
}
