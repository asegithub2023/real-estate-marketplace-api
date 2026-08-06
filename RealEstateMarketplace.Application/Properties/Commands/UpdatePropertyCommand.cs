using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Properties.Commands;

public sealed class UpdatePropertyCommand : IRequest<Result<PropertyDto, PropertyError>>
{
    public int Id { get; set; }
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
    public RealEstateMarketplace.Domain.Enums.PropertyStatus? Status { get; set; }
}
