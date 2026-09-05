namespace RealEstateMarketplace.Domain.Entities;
public class Conversation
{
    public int Id { get; set; }

    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public int BuyerId { get; set; }
    public User Buyer { get; set; } = null!;

    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Message> Messages { get; set; } = [];

}
