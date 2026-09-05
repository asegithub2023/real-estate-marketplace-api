namespace RealEstateMarketplace.Domain.Entities;
public class Notification
{
    public int Id { get; set; }

    public required string Title { get; set; }
    public required string Message { get; set; }

    public bool IsRead { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
