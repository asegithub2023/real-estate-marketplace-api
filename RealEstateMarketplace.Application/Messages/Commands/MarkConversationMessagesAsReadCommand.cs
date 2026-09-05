using MediatR;

namespace RealEstateMarketplace.Application.Messages.Commands;

public sealed class MarkConversationMessagesAsReadCommand : IRequest<int>
{
    public int ConversationId { get; set; }
    public int ReaderUserId { get; set; }
}
