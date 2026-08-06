namespace RealEstateMarketplace.Application.Common;

public sealed record MessageError(string Code, string Message)
{
    public static MessageError NotFound(int id) =>
        new("message_not_found", $"Message '{id}' was not found.");

    public static MessageError ConversationOrSenderNotFound() =>
        new("conversation_or_sender_not_found", "Conversation or sender was not found.");
}
