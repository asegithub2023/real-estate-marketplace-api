using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.PropertyFeatures.Commands;

public sealed class UpdatePropertyFeatureCommand : IRequest<PropertyFeature?>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Icon { get; set; }
}
