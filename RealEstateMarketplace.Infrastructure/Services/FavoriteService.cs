using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Application.Mapping;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Infrastructure.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPropertyRepository _propertyRepository;

    public FavoriteService(IFavoriteRepository favoriteRepository, IUserRepository userRepository, IPropertyRepository propertyRepository)
    {
        _favoriteRepository = favoriteRepository;
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<IReadOnlyList<FavoriteDto>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var favorites = await _favoriteRepository.GetByUserIdAsync(userId, cancellationToken);
        return favorites.Select(favorite => favorite.ToDto()).ToList();
    }

    public async Task<FavoriteDto?> AddAsync(
    int userId,
    int propertyId,
    CancellationToken cancellationToken = default)
{
    var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
    var property = await _propertyRepository.GetByIdAsync(propertyId, cancellationToken);

    if (user is null || property is null)
    {
        throw new InvalidOperationException("User or property was not found.");
    }

    var existingFavorite = await _favoriteRepository.GetByUserAndPropertyAsync(
        userId,
        propertyId,
        cancellationToken);

    if (existingFavorite is not null)
    {
        return existingFavorite.ToDto();
    }

    var favorite = new Favorite
    {
        UserId = userId,
        PropertyId = propertyId
    };

    await _favoriteRepository.AddAsync(favorite, cancellationToken);

    return favorite.ToDto();
}

    public async Task<bool> RemoveAsync(int userId, int propertyId, CancellationToken cancellationToken = default)
    {
        var favorite = await _favoriteRepository.GetByUserAndPropertyAsync(userId, propertyId, cancellationToken);
        if (favorite is null)
        {
            return false;
        }

        await _favoriteRepository.DeleteAsync(userId, propertyId, cancellationToken);
        return true;
    }
}
