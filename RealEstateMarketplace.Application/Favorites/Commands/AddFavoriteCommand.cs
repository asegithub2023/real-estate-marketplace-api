using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Favorites.Commands;

public sealed class AddFavoriteCommand : IRequest<Result<Favorite, FavoriteError>>
{
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}
