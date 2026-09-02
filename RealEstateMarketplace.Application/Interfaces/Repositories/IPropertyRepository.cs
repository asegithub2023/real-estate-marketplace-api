using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Interfaces.Repositories;

public interface IPropertyRepository
{
    Task<Property?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    // Tracked, no navigation includes - used only by the update path. Fetching
    // via GetByIdAsync (AsNoTracking + Include(Owner/Images/Features)) and then
    // calling Update() marks the whole loaded graph as Modified, not just the
    // scalar fields being changed - this is what caused status updates to
    // appear to succeed but not actually persist correctly.
    Task<Property?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Property>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Property>> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Property> Items, int TotalCount)> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task AddAsync(Property property, CancellationToken cancellationToken = default);
    Task UpdateAsync(Property property, CancellationToken cancellationToken = default);
    Task DeleteAsync(Property property, CancellationToken cancellationToken = default);
}