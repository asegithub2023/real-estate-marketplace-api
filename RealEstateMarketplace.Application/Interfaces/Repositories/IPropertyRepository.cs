using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Interfaces.Repositories;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Property>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Property>> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default);
    Task AddAsync(Property property, CancellationToken cancellationToken = default);
    Task UpdateAsync(Property property, CancellationToken cancellationToken = default);
    Task DeleteAsync(Property property, CancellationToken cancellationToken = default);
}
