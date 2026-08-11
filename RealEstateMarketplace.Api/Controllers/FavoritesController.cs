using AutoMapper;
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
    private readonly IMapper _mapper;

    public FavoritesController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet("user/{userId:int}")]
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