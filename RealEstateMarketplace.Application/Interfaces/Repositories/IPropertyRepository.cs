using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Interfaces.Repositories;

public interface IPropertyRepository
{
    // Read queries return detached graphs for safe projection and caching.
    Task<Property?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // Update commands require a tracked entity.
    Task<Property?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Property>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Property>> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Property> Items, int TotalCount)> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task AddAsync(Property property, CancellationToken cancellationToken = default);
    Task UpdateAsync(Property property, CancellationToken cancellationToken = default);
    Task DeleteAsync(Property property, CancellationToken cancellationToken = default);
}
