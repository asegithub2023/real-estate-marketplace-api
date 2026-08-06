using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Mapping;

namespace RealEstateMarketplace.Application.Properties.Queries;

public sealed class GetPropertiesByOwnerIdQueryHandler : IRequestHandler<GetPropertiesByOwnerIdQuery, IReadOnlyList<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertiesByOwnerIdQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<IReadOnlyList<PropertyDto>> Handle(GetPropertiesByOwnerIdQuery request, CancellationToken cancellationToken)
    {
        var properties = await _propertyRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);
        return properties.Select(property => property.ToDto()).ToList();
    }
}
