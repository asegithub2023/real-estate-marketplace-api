using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Interfaces.Repositories;

public interface IPropertyImageRepository
{
    Task<PropertyImage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PropertyImage>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
    Task AddAsync(PropertyImage propertyImage, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
}
