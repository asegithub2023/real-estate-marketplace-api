namespace RealEstateMarketplace.Domain.Entities;
public class PropertyImage
{
    public int Id { get; set; }

    public required string ImageUrl { get; set; }

    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
}
