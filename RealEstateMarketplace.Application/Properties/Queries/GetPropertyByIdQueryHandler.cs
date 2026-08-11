using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Properties.Queries;

public sealed class GetPropertyByIdQueryHandler : IRequestHandler<GetPropertyByIdQuery, Property?>
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertyByIdQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<Property?> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        return await _propertyRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}
