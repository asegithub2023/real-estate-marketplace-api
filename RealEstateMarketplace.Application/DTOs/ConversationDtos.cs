namespace RealEstateMarketplace.Application.DTOs;

public class ConversationDto
{
    public int Id { get; set; }

    public int PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string? PropertyImageUrl { get; set; }

    public int BuyerId { get; set; }
    public int OwnerId { get; set; }

    // The participant who is NOT the current user - resolved server-side so the
    // frontend can render "who am I talking to" without knowing which side it's on.
    public int OtherUserId { get; set; }
    public string OtherUserName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string? LastMessageContent { get; set; }
    public DateTime? LastMessageAt { get; set; }

    // Count of messages in this conversation sent by the OTHER participant that
    // the current user hasn't opened the conversation to read yet.
    public int UnreadCount { get; set; }
}

public class CreateConversationDto
{
    // BuyerId and OwnerId are intentionally NOT here - the buyer is the
    // authenticated user (from the JWT) and the owner is derived from the property.
    public int PropertyId { get; set; }
}

public class UpdateConversationDto
{
    public int? PropertyId { get; set; }
    public int? BuyerId { get; set; }
    public int? OwnerId { get; set; }
}