using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Favorites.Commands;
using RealEstateMarketplace.Application.Favorites.Queries;
using Scalar.AspNetCore;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/favorites")]
[Tags("Favorites")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public FavoritesController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet("user/{userId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<FavoriteDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Get favorites for a user")]
    [EndpointDescription("Returns the list of favorite properties for the specified user.")]
    public async Task<ActionResult<IReadOnlyList<FavoriteDto>>> GetByUserId(
        int userId,
        CancellationToken cancellationToken)
    {
        var favorites = await _sender.Send(
            new GetUserFavoritesQuery { UserId = userId },
            cancellationToken);

        return Ok(_mapper.Map<IReadOnlyList<FavoriteDto>>(favorites));
    }

    [HttpPost]
    [ProducesResponseType(typeof(FavoriteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Add a favorite property")]
    [EndpointDescription("Creates a new favorite entry for the authenticated user.")]
    public async Task<ActionResult<FavoriteDto>> Create(
        [FromBody] CreateFavoriteDto request,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<AddFavoriteCommand>(request);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetByUserId),
                new { userId = result.Value!.UserId },
                _mapper.Map<FavoriteDto>(result.Value))
            : result.Error!.Code == "user_or_property_not_found"
                ? NotFound(result.Error.Message)
                : BadRequest(result.Error.Message);
    }

    [HttpDelete("user/{userId:int}/property/{propertyId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Remove a favorite property")]
    [EndpointDescription("Deletes the favorite entry for the given user and property.")]
    public async Task<ActionResult> Delete(
        int userId,
        int propertyId,
        CancellationToken cancellationToken)
    {
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