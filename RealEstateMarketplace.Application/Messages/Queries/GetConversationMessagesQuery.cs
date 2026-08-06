using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Messages.Queries;

public sealed class GetConversationMessagesQuery : IRequest<IReadOnlyList<MessageDto>>
{
    public int ConversationId { get; set; }
}
