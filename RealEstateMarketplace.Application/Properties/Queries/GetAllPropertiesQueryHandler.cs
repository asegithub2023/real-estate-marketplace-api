using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Mapping;

namespace RealEstateMarketplace.Application.Properties.Queries;

public sealed class GetAllPropertiesQueryHandler : IRequestHandler<GetAllPropertiesQuery, IReadOnlyList<PropertyDto>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetAllPropertiesQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<IReadOnlyList<PropertyDto>> Handle(GetAllPropertiesQuery request, CancellationToken cancellationToken)
    {
        var properties = await _propertyRepository.GetAllAsync(cancellationToken);
        return properties.Select(property => property.ToDto()).ToList();
    }
}
