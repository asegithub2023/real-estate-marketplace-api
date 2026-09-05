using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Interfaces.Services;

public interface IFavoriteService
{
    Task<IReadOnlyList<FavoriteDto>> GetByUserIdAsync(
        int userId,
        CancellationToken cancellationToken = default);

    Task<FavoriteDto?> AddAsync(
        int userId,
        int propertyId,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(
        int userId,
        int propertyId,
        CancellationToken cancellationToken = default);
}
