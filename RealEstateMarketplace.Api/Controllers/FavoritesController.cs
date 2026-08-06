using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.Common;
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
        var result = await _sender.Send(new AddFavoriteCommand
        {
            UserId = request.UserId,
            PropertyId = request.PropertyId
        }, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetByUserId), new { userId = result.Value!.UserId }, result.Value)
            : result.Error!.Code == "user_or_property_not_found"
                ? NotFound(result.Error.Message)
                : BadRequest(result.Error.Message);
    }

    [HttpDelete("user/{userId:int}/property/{propertyId:int}")]
    public async Task<ActionResult> Delete(int userId, int propertyId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RemoveFavoriteCommand { UserId = userId, PropertyId = propertyId }, cancellationToken);
        return result.IsSuccess ? NoContent() : NotFound();
    }
}
