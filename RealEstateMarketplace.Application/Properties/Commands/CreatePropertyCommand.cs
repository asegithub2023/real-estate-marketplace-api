using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Properties.Commands;

public sealed class CreatePropertyCommand : IRequest<Result<Property, PropertyError>>
{
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
    public RealEstateMarketplace.Domain.Enums.PropertyStatus Status { get; set; }
    public int OwnerId { get; set; }
}
