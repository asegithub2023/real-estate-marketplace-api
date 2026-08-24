namespace RealEstateMarketplace.Application.Common;

public sealed record ConversationError(string Code, string Message)
{
    public static ConversationError NotFound(int id) =>
        new("conversation_not_found", $"Conversation '{id}' was not found.");

    public static ConversationError UserOrPropertyNotFound() =>
        new("user_or_property_not_found", "Buyer, owner, or property was not found.");

    public static ConversationError CannotContactOwnProperty() =>
        new("cannot_contact_own_property", "You cannot start a conversation about your own property.");
}