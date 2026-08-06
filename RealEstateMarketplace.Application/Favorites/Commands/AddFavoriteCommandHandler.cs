using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Favorites.Commands;

public sealed class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, Result<FavoriteDto, FavoriteError>>
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

    public async Task<Result<FavoriteDto, FavoriteError>> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);

        if (user is null || property is null)
        {
            return Result.Failure<FavoriteDto, FavoriteError>(FavoriteError.UserOrPropertyNotFound());
        }

        var existingFavorite = await _favoriteRepository.GetByUserAndPropertyAsync(request.UserId, request.PropertyId, cancellationToken);
        if (existingFavorite is not null)
        {
            return Result.Success<FavoriteDto, FavoriteError>(new FavoriteDto
            {
                Id = existingFavorite.Id,
                UserId = existingFavorite.UserId,
                PropertyId = existingFavorite.PropertyId
            });
        }

        var favorite = new Favorite
        {
            UserId = request.UserId,
            PropertyId = request.PropertyId
        };

        await _favoriteRepository.AddAsync(favorite, cancellationToken);

        return Result.Success<FavoriteDto, FavoriteError>(new FavoriteDto
        {
            Id = favorite.Id,
            UserId = favorite.UserId,
            PropertyId = favorite.PropertyId
        });
    }
}
