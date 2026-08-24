namespace RealEstateMarketplace.Application.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}

public class CreateMessageDto
{
    // SenderId is intentionally NOT here - the sender is always the authenticated
    // user (from the JWT), never a value the client supplies.
    public int ConversationId { get; set; }
    public required string Content { get; set; }
}

public class UpdateMessageDto
{
    public string? Content { get; set; }
}