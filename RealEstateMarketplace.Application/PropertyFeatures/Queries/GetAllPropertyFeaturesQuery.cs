using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.PropertyFeatures.Queries;

public sealed class GetAllPropertyFeaturesQuery : IRequest<IReadOnlyList<PropertyFeature>>
{
}
