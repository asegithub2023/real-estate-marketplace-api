using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using RealEstateMarketplace.Domain.Enums;

namespace RealEstateMarketplace.Application.DTOs;

public class PropertyDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Rooms { get; set; }
    public double Area { get; set; }
    public PropertyStatus Status { get; set; }
    public PropertyType PropertyType { get; set; }
    public ListingType ListingType { get; set; }
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public List<PropertyImageDto> Images { get; set; } = [];
    public List<PropertyFeatureDto> Features { get; set; } = [];
}

public class CreatePropertyDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Rooms { get; set; }
    public double Area { get; set; }
    public PropertyType PropertyType { get; set; }
    public ListingType ListingType { get; set; }
    public int OwnerId { get; set; }

    [Required(ErrorMessage = "At least one image is required.")]
    [MinLength(1, ErrorMessage = "At least one image is required.")]
    [MaxLength(7, ErrorMessage = "You can upload a maximum of 7 images.")]
    public List<IFormFile> Images { get; set; } = [];
}

public class UpdatePropertyDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? Country { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public int? Rooms { get; set; }
    public double? Area { get; set; }
    public PropertyStatus? Status { get; set; }
}