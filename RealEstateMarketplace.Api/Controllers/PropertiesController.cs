using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.Common;
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
    private readonly IMapper _mapper;

    public PropertiesController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<PropertyDto>>> GetAll(CancellationToken cancellationToken)
    {
        var properties = await _sender.Send(new GetAllPropertiesQuery(), cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<PropertyDto>>(properties));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<PropertyDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var property = await _sender.Send(new GetPropertyByIdQuery { Id = id }, cancellationToken);
        return property is null ? NotFound() : Ok(_mapper.Map<PropertyDto>(property));
    }

    [HttpGet("owner/{ownerId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<PropertyDto>>> GetByOwnerId(int ownerId, CancellationToken cancellationToken)
    {
        var properties = await _sender.Send(new GetPropertiesByOwnerIdQuery { OwnerId = ownerId }, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<PropertyDto>>(properties));
    }

    [HttpPost]
    [AllowAnonymous]
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

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : result.Error!.Code == "owner_not_found"
                ? NotFound(result.Error.Message)
                : BadRequest(result.Error.Message);
    }

    [HttpPut("{id:int}")]
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

        return result.IsSuccess ? Ok(result.Value!) : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeletePropertyCommand { Id = id }, cancellationToken);
        return result.IsSuccess ? NoContent() : NotFound();
    }
}
