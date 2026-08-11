using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Favorites.Queries;

public sealed class GetUserFavoritesQuery : IRequest<IReadOnlyList<Favorite>>
{
    public int UserId { get; set; }
}
