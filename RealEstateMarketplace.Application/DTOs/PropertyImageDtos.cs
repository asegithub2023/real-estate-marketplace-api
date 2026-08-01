namespace RealEstateMarketplace.Application.DTOs;

public class PropertyImageDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int PropertyId { get; set; }
}

public class CreatePropertyImageDto
{
    public required string ImageUrl { get; set; }
    public int PropertyId { get; set; }
}

public class UpdatePropertyImageDto
{
    public string? ImageUrl { get; set; }
}
