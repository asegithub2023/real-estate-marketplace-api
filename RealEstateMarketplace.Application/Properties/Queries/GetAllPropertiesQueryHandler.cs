using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Properties.Queries;

public sealed class GetAllPropertiesQueryHandler : IRequestHandler<GetAllPropertiesQuery, IReadOnlyList<Property>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetAllPropertiesQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<IReadOnlyList<Property>> Handle(GetAllPropertiesQuery request, CancellationToken cancellationToken)
    {
        return await _propertyRepository.GetAllAsync(cancellationToken);
    }
}
