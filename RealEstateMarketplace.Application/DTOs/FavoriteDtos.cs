namespace RealEstateMarketplace.Application.DTOs;

public class FavoriteDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public decimal PropertyPrice { get; set; }
    public string PropertyCity { get; set; } = string.Empty;
    public string PropertyCountry { get; set; } = string.Empty;
    public int PropertyBedrooms { get; set; }
    public int PropertyBathrooms { get; set; }
    public double PropertyArea { get; set; }
    public string? PropertyImageUrl { get; set; }
}