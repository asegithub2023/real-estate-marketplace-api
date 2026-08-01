namespace RealEstateMarketplace.Application.DTOs;

public class PropertyFeatureDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class CreatePropertyFeatureDto
{
    public required string Name { get; set; }
    public string Icon { get; set; } = string.Empty;
}

public class UpdatePropertyFeatureDto
{
    public string? Name { get; set; }
    public string? Icon { get; set; }
}
