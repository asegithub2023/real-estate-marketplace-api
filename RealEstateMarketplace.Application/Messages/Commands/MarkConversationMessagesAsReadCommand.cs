using MediatR;

namespace RealEstateMarketplace.Application.Messages.Commands;

/// Marks every message in the conversation not sent by ReaderUserId as read.
/// Returns the number of messages that were newly marked (0 if already caught up).
public sealed class MarkConversationMessagesAsReadCommand : IRequest<int>
{
    public int ConversationId { get; set; }
    public int ReaderUserId { get; set; }
}