namespace RealEstateMarketplace.Domain.Entities;
public class PropertyFeature
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;

    public ICollection<Property> Properties { get; set; } = [];
}