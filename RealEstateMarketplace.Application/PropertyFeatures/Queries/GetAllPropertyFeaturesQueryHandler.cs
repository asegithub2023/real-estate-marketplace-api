using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.PropertyFeatures.Queries;

public sealed class GetAllPropertyFeaturesQueryHandler : IRequestHandler<GetAllPropertyFeaturesQuery, IReadOnlyList<PropertyFeature>>
{
    private readonly IPropertyFeatureRepository _repository;

    public GetAllPropertyFeaturesQueryHandler(IPropertyFeatureRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PropertyFeature>> Handle(GetAllPropertyFeaturesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}
