using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.PropertyFeatures.Commands;

public sealed class CreatePropertyFeatureCommand : IRequest<PropertyFeature>
{
    public required string Name { get; set; }
    public string Icon { get; set; } = string.Empty;
}
