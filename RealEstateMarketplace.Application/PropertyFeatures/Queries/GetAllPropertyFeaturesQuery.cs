using MediatR;
using RealEstateMarketplace.Application.DTOs;
using System.Collections.Generic;

namespace RealEstateMarketplace.Application.PropertyFeatures.Queries;

public sealed class GetAllPropertyFeaturesQuery : IRequest<IReadOnlyList<PropertyFeatureDto>>
{
}
