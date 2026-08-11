using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Favorites.Queries;

public sealed class GetUserFavoritesQueryHandler : IRequestHandler<GetUserFavoritesQuery, IReadOnlyList<Favorite>>
{
    private readonly IFavoriteRepository _favoriteRepository;

    public GetUserFavoritesQueryHandler(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public async Task<IReadOnlyList<Favorite>> Handle(GetUserFavoritesQuery request, CancellationToken cancellationToken)
    {
        return await _favoriteRepository.GetByUserIdAsync(request.UserId, cancellationToken);
    }
}
