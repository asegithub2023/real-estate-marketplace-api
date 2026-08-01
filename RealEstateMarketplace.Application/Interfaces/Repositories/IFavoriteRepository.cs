using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Interfaces.Repositories;

public interface IFavoriteRepository
{
    Task<Favorite?> GetByUserAndPropertyAsync(int userId, int propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Favorite>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task AddAsync(Favorite favorite, CancellationToken cancellationToken = default);
    Task DeleteAsync(int userId, int propertyId, CancellationToken cancellationToken = default);
}
