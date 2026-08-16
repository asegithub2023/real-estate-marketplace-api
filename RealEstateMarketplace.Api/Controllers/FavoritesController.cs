using AutoMapper;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Api.Utilities;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Favorites.Commands;
using RealEstateMarketplace.Application.Favorites.Queries;
using Scalar.AspNetCore;

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

    [HttpGet("user/{userId:int}")]
    [ProducesResponseType(typeof(List<HateoasResponse<FavoriteDto>>), StatusCodes.Status200OK)]
    [EndpointSummary("Get favorites for a user")]
    [EndpointDescription("Returns the list of favorite properties for the specified user with HATEOAS links.")]
    public async Task<ActionResult<List<HateoasResponse<FavoriteDto>>>> GetUserFavorites(
        int userId,
        CancellationToken cancellationToken)
    {
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

    [HttpPost]
    [ProducesResponseType(typeof(HateoasResponse<FavoriteDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Add a favorite property")]
    [EndpointDescription("Creates a new favorite entry for the authenticated user with HATEOAS links.")]
    public async Task<ActionResult<HateoasResponse<FavoriteDto>>> AddFavorite(
        [FromBody] CreateFavoriteDto request,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<AddFavoriteCommand>(request);

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
            new { userId = result.Value!.UserId },
            response);
    }

    [HttpDelete("{propertyId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Remove a favorite property")]
    [EndpointDescription("Deletes the favorite entry for the authenticated user and specified property.")]
    public async Task<ActionResult> RemoveFavorite(
        int propertyId,
        CancellationToken cancellationToken)
    {
        // Get user ID from claims (assuming it's stored in token)
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("nameid");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
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