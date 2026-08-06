using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.PropertyFeatures.Commands;

public sealed class UpdatePropertyFeatureCommand : IRequest<PropertyFeatureDto?>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Icon { get; set; }
}
