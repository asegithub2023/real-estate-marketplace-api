using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Favorites.Commands;

public sealed class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, Result<bool, FavoriteError>>
{
    private readonly IFavoriteRepository _favoriteRepository;

    public RemoveFavoriteCommandHandler(IFavoriteRepository favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }

    public async Task<Result<bool, FavoriteError>> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
    {
        var favorite = await _favoriteRepository.GetByUserAndPropertyAsync(request.UserId, request.PropertyId, cancellationToken);
        if (favorite is null)
        {
            return Result.Failure<bool, FavoriteError>(FavoriteError.NotFound(request.PropertyId));
        }

        await _favoriteRepository.DeleteAsync(request.UserId, request.PropertyId, cancellationToken);
        return Result.Success<bool, FavoriteError>(true);
    }
}
