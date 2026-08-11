using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.PropertyFeatures.Queries;

public sealed class GetPropertyFeatureByIdQuery : IRequest<PropertyFeature?>
{
    public int Id { get; set; }
}
