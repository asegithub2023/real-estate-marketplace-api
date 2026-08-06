using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Favorites.Commands;

public sealed class AddFavoriteCommand : IRequest<FavoriteDto?>
{
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}
