using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Favorites.Queries;

public sealed class GetUserFavoritesQuery : IRequest<IReadOnlyList<FavoriteDto>>
{
    public int UserId { get; set; }
}
