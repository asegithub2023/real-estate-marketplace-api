using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Properties.Commands;
using RealEstateMarketplace.Application.Properties.Queries;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PropertiesController : ControllerBase
{
    private readonly ISender _sender;

    public PropertiesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<PropertyDto>>> GetAll(CancellationToken cancellationToken)
    {
        var properties = await _sender.Send(new GetAllPropertiesQuery(), cancellationToken);
        return Ok(properties);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<PropertyDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var property = await _sender.Send(new GetPropertyByIdQuery { Id = id }, cancellationToken);
        return property is null ? NotFound() : Ok(property);
    }

    [HttpGet("owner/{ownerId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<PropertyDto>>> GetByOwnerId(int ownerId, CancellationToken cancellationToken)
    {
        var properties = await _sender.Send(new GetPropertiesByOwnerIdQuery { OwnerId = ownerId }, cancellationToken);
        return Ok(properties);
    }

    [HttpPost]
    public async Task<ActionResult<PropertyDto>> Create([FromBody] CreatePropertyDto request, CancellationToken cancellationToken)
    {
        var property = await _sender.Send(new CreatePropertyCommand
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

        return CreatedAtAction(nameof(GetById), new { id = property.Id }, property);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PropertyDto>> Update(int id, [FromBody] UpdatePropertyDto request, CancellationToken cancellationToken)
    {
        var property = await _sender.Send(new UpdatePropertyCommand
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

        return property is null ? NotFound() : Ok(property);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeletePropertyCommand { Id = id }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
