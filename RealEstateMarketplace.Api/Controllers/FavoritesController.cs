using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Favorites.Commands;
using RealEstateMarketplace.Application.Favorites.Queries;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly ISender _sender;

    public FavoritesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IReadOnlyList<FavoriteDto>>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        var favorites = await _sender.Send(new GetUserFavoritesQuery { UserId = userId }, cancellationToken);
        return Ok(favorites);
    }

    [HttpPost]
    public async Task<ActionResult<FavoriteDto>> Create([FromBody] CreateFavoriteDto request, CancellationToken cancellationToken)
    {
        var favorite = await _sender.Send(new AddFavoriteCommand
        {
            UserId = request.UserId,
            PropertyId = request.PropertyId
        }, cancellationToken);

        return favorite is null ? BadRequest() : CreatedAtAction(nameof(GetByUserId), new { userId = favorite.UserId }, favorite);
    }

    [HttpDelete("user/{userId:int}/property/{propertyId:int}")]
    public async Task<ActionResult> Delete(int userId, int propertyId, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new RemoveFavoriteCommand { UserId = userId, PropertyId = propertyId }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
