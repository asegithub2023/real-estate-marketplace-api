namespace RealEstateMarketplace.Domain.Entities;
public class Message
{
    public int Id { get; set; }

    public int ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;

    public int SenderId { get; set; }
    public User Sender { get; set; } = null!;

    public string Content { get; set; } = string.Empty;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    // True once the recipient (the participant who is NOT the sender) has opened
    // the conversation. Used only to compute ConversationDto.UnreadCount - there's
    // no per-message "seen" UI.
    public bool IsRead { get; set; }
}