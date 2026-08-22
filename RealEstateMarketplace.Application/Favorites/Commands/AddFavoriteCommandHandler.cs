using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Favorites.Commands;

public sealed class AddFavoriteCommandHandler
    : IRequestHandler<AddFavoriteCommand, Result<Favorite, FavoriteError>>
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPropertyRepository _propertyRepository;

    public AddFavoriteCommandHandler(
        IFavoriteRepository favoriteRepository,
        IUserRepository userRepository,
        IPropertyRepository propertyRepository)
    {
        _favoriteRepository = favoriteRepository;
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<Favorite, FavoriteError>> Handle(
        AddFavoriteCommand request,
        CancellationToken cancellationToken)
    {
        // Make sure the user exists
        var user = await _userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        // Make sure the property exists
        var property = await _propertyRepository.GetByIdAsync(
            request.PropertyId,
            cancellationToken);

        if (user is null || property is null)
        {
            return Result.Failure<Favorite, FavoriteError>(
                FavoriteError.UserOrPropertyNotFound());
        }

        // Check whether the favorite already exists
        var existingFavorite =
            await _favoriteRepository.GetByUserAndPropertyAsync(
                request.UserId,
                request.PropertyId,
                cancellationToken);

        if (existingFavorite is not null)
        {
            return Result.Success<Favorite, FavoriteError>(
                existingFavorite);
        }

        // Only set the foreign keys.
        // Do NOT set User or Property navigation properties.
        var favorite = new Favorite
        {
            UserId = request.UserId,
            PropertyId = request.PropertyId
        };

        await _favoriteRepository.AddAsync(
            favorite,
            cancellationToken);

        return Result.Success<Favorite, FavoriteError>(
            favorite);
    }
}