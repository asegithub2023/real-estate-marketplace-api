using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.PropertyFeatures.Commands;

public sealed class CreatePropertyFeatureCommand : IRequest<PropertyFeatureDto>
{
    public required string Name { get; set; }
    public string Icon { get; set; } = string.Empty;
}
