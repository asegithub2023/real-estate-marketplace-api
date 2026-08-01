namespace RealEstateMarketplace.Application.DTOs;

public class ConversationDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public int BuyerId { get; set; }
    public int OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateConversationDto
{
    public int PropertyId { get; set; }
    public int BuyerId { get; set; }
    public int OwnerId { get; set; }
}

public class UpdateConversationDto
{
    public int? PropertyId { get; set; }
    public int? BuyerId { get; set; }
    public int? OwnerId { get; set; }
}
