using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Favorites.Commands;

public sealed class AddFavoriteCommand : IRequest<Result<FavoriteDto, FavoriteError>>
{
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}
