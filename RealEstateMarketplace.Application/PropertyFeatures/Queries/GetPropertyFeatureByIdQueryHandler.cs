using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.PropertyFeatures.Queries;

public sealed class GetPropertyFeatureByIdQueryHandler : IRequestHandler<GetPropertyFeatureByIdQuery, PropertyFeature?>
{
    private readonly IPropertyFeatureRepository _repository;

    public GetPropertyFeatureByIdQueryHandler(IPropertyFeatureRepository repository)
    {
        _repository = repository;
    }

    public async Task<PropertyFeature?> Handle(GetPropertyFeatureByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.Id, cancellationToken);
    }
}
