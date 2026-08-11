using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Properties.Queries;

public sealed class GetPropertiesByOwnerIdQueryHandler : IRequestHandler<GetPropertiesByOwnerIdQuery, IReadOnlyList<Property>>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertiesByOwnerIdQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<IReadOnlyList<Property>> Handle(GetPropertiesByOwnerIdQuery request, CancellationToken cancellationToken)
    {
        return await _propertyRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);
    }
}
