using AutoMapper;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using RealEstateMarketplace.Api.Utilities;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Favorites.Commands;
using RealEstateMarketplace.Application.Favorites.Queries;
using System.Security.Claims;
namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Tags("Favorites")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
[ApiVersion("1.0")]
public class FavoritesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    private readonly IHateoasHelper _hateoasHelper;

    public FavoritesController(ISender sender, IMapper mapper, IHateoasHelper hateoasHelper)
    {
        _sender = sender;
        _mapper = mapper;
        _hateoasHelper = hateoasHelper;
    }

    // =========================================================
    // Resolve the current user strictly from the JWT.
    // The API's JwtBearer handler uses the default inbound claim
    // mapping (Program.cs does not set MapInboundClaims = false),
    // so the "sub" claim TokenService issues arrives here already
    // remapped to ClaimTypes.NameIdentifier. Angular never supplies
    // this value - it is never read from the route, query, or body.
    // =========================================================
   private bool TryGetCurrentUserId(out int userId)
{
    var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
    return int.TryParse(value, out userId);
}
    
    [HttpGet("me")]
    [ProducesResponseType(typeof(List<HateoasResponse<FavoriteDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Get favorites for the current user")]
    [EndpointDescription("Returns the list of favorite properties for the authenticated user with HATEOAS links.")]
    public async Task<ActionResult<List<HateoasResponse<FavoriteDto>>>> GetUserFavorites(
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var favorites = await _sender.Send(
            new GetUserFavoritesQuery { UserId = userId },
            cancellationToken);

        var favoriteDtos = _mapper.Map<IReadOnlyList<FavoriteDto>>(favorites);
        var response = favoriteDtos.Select(f => new HateoasResponse<FavoriteDto>
        {
            Data = f,
            Links = _hateoasHelper.GenerateFavoriteResourceLinks(f.PropertyId, userId)
        }).ToList();

        return Ok(response);
    }

    [HttpPost("{propertyId:int}")]
    [ProducesResponseType(typeof(HateoasResponse<FavoriteDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Add a favorite property")]
    [EndpointDescription("Creates a new favorite entry for the authenticated user with HATEOAS links.")]
    public async Task<ActionResult<HateoasResponse<FavoriteDto>>> AddFavorite(
        int propertyId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var command = new AddFavoriteCommand
        {
            UserId = userId,
            PropertyId = propertyId
        };

        var result = await _sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error!.Code == "user_or_property_not_found"
                ? NotFound(result.Error.Message)
                : BadRequest(result.Error.Message);
        }

        var favoriteDto = _mapper.Map<FavoriteDto>(result.Value);
        var response = new HateoasResponse<FavoriteDto>
        {
            Data = favoriteDto,
            Links = _hateoasHelper.GenerateFavoriteResourceLinks(favoriteDto.PropertyId, favoriteDto.UserId)
        };

        return CreatedAtAction(
            nameof(GetUserFavorites),
            response);
    }

    [HttpDelete("{propertyId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Remove a favorite property")]
    [EndpointDescription("Deletes the favorite entry for the authenticated user and specified property.")]
    public async Task<ActionResult> RemoveFavorite(
        int propertyId,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await _sender.Send(
            new RemoveFavoriteCommand
            {
                UserId = userId,
                PropertyId = propertyId
            },
            cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : NotFound();
    }
}