using AutoMapper;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Infrastructure.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IMapper _mapper;

    public FavoriteService(IFavoriteRepository favoriteRepository, IUserRepository userRepository, IPropertyRepository propertyRepository, IMapper mapper)
    {
        _favoriteRepository = favoriteRepository;
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<FavoriteDto>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var favorites = await _favoriteRepository.GetByUserIdAsync(userId, cancellationToken);
        return favorites.Select(favorite => _mapper.Map<FavoriteDto>(favorite)).ToList();
    }

    public async Task<FavoriteDto?> AddAsync(CreateFavoriteDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (user is null || property is null)
        {
            throw new InvalidOperationException("User or property was not found.");
        }

        var existingFavorite = await _favoriteRepository.GetByUserAndPropertyAsync(request.UserId, request.PropertyId, cancellationToken);
        if (existingFavorite is not null)
        {
            return _mapper.Map<FavoriteDto>(existingFavorite);
        }

        var favorite = new Favorite
        {
            UserId = request.UserId,
            PropertyId = request.PropertyId
        };

        await _favoriteRepository.AddAsync(favorite, cancellationToken);
        return _mapper.Map<FavoriteDto>(favorite);
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
