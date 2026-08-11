using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Messages.Queries;

public sealed class GetConversationMessagesQuery : IRequest<IReadOnlyList<Message>>
{
    public int ConversationId { get; set; }
}
