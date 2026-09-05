namespace RealEstateMarketplace.Domain.Entities;
public class Favorite
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
}
