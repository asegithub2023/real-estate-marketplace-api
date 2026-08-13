using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Application.Properties.Commands;
using RealEstateMarketplace.Application.Properties.Queries;
using Scalar.AspNetCore;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/properties")]
[Tags("Properties")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
public class PropertiesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    private readonly ICachedPropertyService _cachedPropertyService;
    private readonly IPropertyService _propertyService;

    public PropertiesController(
        ISender sender,
        IMapper mapper,
        ICachedPropertyService cachedPropertyService,
        IPropertyService propertyService)
    {
        _sender = sender;
        _mapper = mapper;
        _cachedPropertyService = cachedPropertyService;
        _propertyService = propertyService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<PropertyDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Get all properties")]
    [EndpointDescription("Returns all available properties.")]
    public async Task<ActionResult<IReadOnlyList<PropertyDto>>> GetAll(CancellationToken cancellationToken)
    {
        var properties = await _cachedPropertyService.GetAllPropertiesAsync(cancellationToken);
        return Ok(properties);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<PropertyDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Search and filter properties")]
    [EndpointDescription("Returns paginated properties with search, filtering, and sorting capabilities.")]
    public async Task<ActionResult<PagedResponse<PropertyDto>>> Search([FromQuery] PagedRequest request, CancellationToken cancellationToken)
    {
        var result = await _propertyService.GetPropertiesAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get a property by ID")]
    [EndpointDescription("Returns the property matching the specified identifier.")]
    public async Task<ActionResult<PropertyDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var property = await _cachedPropertyService.GetPropertyAsync(id, cancellationToken);
        return property is null ? NotFound() : Ok(property);
    }

    [HttpGet("owner/{ownerId:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<PropertyDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Get properties by owner")]
    [EndpointDescription("Returns all properties owned by the specified user.")]
    public async Task<ActionResult<IReadOnlyList<PropertyDto>>> GetByOwnerId(int ownerId, CancellationToken cancellationToken)
    {
        var properties = await _sender.Send(new GetPropertiesByOwnerIdQuery { OwnerId = ownerId }, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<PropertyDto>>(properties));
    }

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Create a property")]
    [EndpointDescription("Creates a new property record.")]
    public async Task<ActionResult<PropertyDto>> Create([FromBody] CreatePropertyDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreatePropertyCommand
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            City = request.City,
            Address = request.Address,
            Country = request.Country,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            Rooms = request.Rooms,
            Area = request.Area,
            Status = request.Status,
            OwnerId = request.OwnerId
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error!.Code == "owner_not_found"
                ? NotFound(result.Error.Message)
                : BadRequest(result.Error.Message);
        }

        await _cachedPropertyService.InvalidatePropertyCacheAsync(null, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Update a property")]
    [EndpointDescription("Updates an existing property by ID.")]
    public async Task<ActionResult<PropertyDto>> Update(int id, [FromBody] UpdatePropertyDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdatePropertyCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            City = request.City,
            Address = request.Address,
            Country = request.Country,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            Rooms = request.Rooms,
            Area = request.Area,
            Status = request.Status
        }, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound();
        }

        await _cachedPropertyService.InvalidatePropertyCacheAsync(id, cancellationToken);
        return Ok(result.Value!);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Delete a property")]
    [EndpointDescription("Deletes the specified property.")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeletePropertyCommand { Id = id }, cancellationToken);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        await _cachedPropertyService.InvalidatePropertyCacheAsync(id, cancellationToken);
        return NoContent();
    }
}
