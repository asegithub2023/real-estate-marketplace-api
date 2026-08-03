using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Services;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IReadOnlyList<FavoriteDto>>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        var favorites = await _favoriteService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(favorites);
    }

    [HttpPost]
    public async Task<ActionResult<FavoriteDto>> Create([FromBody] CreateFavoriteDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var favorite = await _favoriteService.AddAsync(request, cancellationToken);
        return favorite is null ? BadRequest() : CreatedAtAction(nameof(GetByUserId), new { userId = favorite.UserId }, favorite);
    }

    [HttpDelete("user/{userId:int}/property/{propertyId:int}")]
    public async Task<ActionResult> Delete(int userId, int propertyId, CancellationToken cancellationToken)
    {
        var deleted = await _favoriteService.RemoveAsync(userId, propertyId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
