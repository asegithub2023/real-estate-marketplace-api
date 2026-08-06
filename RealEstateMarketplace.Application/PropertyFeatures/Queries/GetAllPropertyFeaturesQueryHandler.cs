using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Mapping;

namespace RealEstateMarketplace.Application.PropertyFeatures.Queries;

public sealed class GetAllPropertyFeaturesQueryHandler : IRequestHandler<GetAllPropertyFeaturesQuery, IReadOnlyList<PropertyFeatureDto>>
{
    private readonly IPropertyFeatureRepository _repository;

    public GetAllPropertyFeaturesQueryHandler(IPropertyFeatureRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PropertyFeatureDto>> Handle(GetAllPropertyFeaturesQuery request, CancellationToken cancellationToken)
    {
        var features = await _repository.GetAllAsync(cancellationToken);
        return features.Select(feature => feature.ToDto()).ToList();
    }
}
