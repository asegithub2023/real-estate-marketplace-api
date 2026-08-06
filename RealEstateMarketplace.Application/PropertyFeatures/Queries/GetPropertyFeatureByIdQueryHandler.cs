using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Mapping;

namespace RealEstateMarketplace.Application.PropertyFeatures.Queries;

public sealed class GetPropertyFeatureByIdQueryHandler : IRequestHandler<GetPropertyFeatureByIdQuery, PropertyFeatureDto?>
{
    private readonly IPropertyFeatureRepository _repository;

    public GetPropertyFeatureByIdQueryHandler(IPropertyFeatureRepository repository)
    {
        _repository = repository;
    }

    public async Task<PropertyFeatureDto?> Handle(GetPropertyFeatureByIdQuery request, CancellationToken cancellationToken)
    {
        var feature = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return feature is null ? null : feature.ToDto();
    }
}
