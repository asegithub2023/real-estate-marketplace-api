using MediatR;
using RealEstateMarketplace.Application.Common;

namespace RealEstateMarketplace.Application.Favorites.Commands;

public sealed class RemoveFavoriteCommand : IRequest<Result<bool, FavoriteError>>
{
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}
