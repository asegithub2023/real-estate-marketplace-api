namespace RealEstateMarketplace.Application.DTOs;

public class ConversationDto
{
    public int Id { get; set; }

    public int PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string? PropertyImageUrl { get; set; }

    public int BuyerId { get; set; }
    public int OwnerId { get; set; }

    public int OtherUserId { get; set; }
    public string OtherUserName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string? LastMessageContent { get; set; }
    public DateTime? LastMessageAt { get; set; }

    public int UnreadCount { get; set; }
}

public class CreateConversationDto
{

    public int PropertyId { get; set; }
}

public class UpdateConversationDto
{
    public int? PropertyId { get; set; }
    public int? BuyerId { get; set; }
    public int? OwnerId { get; set; }
}
