using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.PropertyFeatures.Queries;

public sealed class GetPropertyFeatureByIdQuery : IRequest<PropertyFeatureDto?>
{
    public int Id { get; set; }
}
