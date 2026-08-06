using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Favorites.Commands;

public sealed class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, bool>
{
    private readonly IFavoriteRepository _favoriteRepository;

    public RemoveFavoriteCommandHandler(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public async Task<bool> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
    {
        var favorite = await _favoriteRepository.GetByUserAndPropertyAsync(request.UserId, request.PropertyId, cancellationToken);
        if (favorite is null)
        {
            return false;
        }

        await _favoriteRepository.DeleteAsync(request.UserId, request.PropertyId, cancellationToken);
        return true;
    }
}
