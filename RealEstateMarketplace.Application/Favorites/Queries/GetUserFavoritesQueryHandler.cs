using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Favorites.Queries;

public sealed class GetUserFavoritesQueryHandler : IRequestHandler<GetUserFavoritesQuery, IReadOnlyList<FavoriteDto>>
{
    private readonly IFavoriteRepository _favoriteRepository;

    public GetUserFavoritesQueryHandler(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public async Task<IReadOnlyList<FavoriteDto>> Handle(GetUserFavoritesQuery request, CancellationToken cancellationToken)
    {
        var favorites = await _favoriteRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        return favorites.Select(favorite => new FavoriteDto
        {
            Id = favorite.Id,
            UserId = favorite.UserId,
            PropertyId = favorite.PropertyId
        }).ToList();
    }
}
