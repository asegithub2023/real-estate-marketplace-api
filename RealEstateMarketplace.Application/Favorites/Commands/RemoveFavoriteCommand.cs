using MediatR;

namespace RealEstateMarketplace.Application.Favorites.Commands;

public sealed class RemoveFavoriteCommand : IRequest<bool>
{
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}
