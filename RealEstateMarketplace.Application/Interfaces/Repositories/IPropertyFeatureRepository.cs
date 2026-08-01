using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Interfaces.Repositories;

public interface IPropertyFeatureRepository
{
    Task<PropertyFeature?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PropertyFeature>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PropertyFeature?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(PropertyFeature feature, CancellationToken cancellationToken = default);
    Task UpdateAsync(PropertyFeature feature, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
