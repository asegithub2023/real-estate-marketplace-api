using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.PropertyFeatures.Commands;
using RealEstateMarketplace.Application.PropertyFeatures.Queries;
using Scalar.AspNetCore;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/features")]
[Tags("PropertyFeatures")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
public class FeaturesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public FeaturesController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<PropertyFeatureDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Get all property features")]
    [EndpointDescription("Returns all available property features.")]
    public async Task<ActionResult<IReadOnlyList<PropertyFeatureDto>>> GetAll(CancellationToken cancellationToken)
    {
        var features = await _sender.Send(new GetAllPropertyFeaturesQuery(), cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<PropertyFeatureDto>>(features));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PropertyFeatureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get a property feature by ID")]
    [EndpointDescription("Returns the property feature matching the specified identifier.")]
    public async Task<ActionResult<PropertyFeatureDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var feature = await _sender.Send(new GetPropertyFeatureByIdQuery { Id = id }, cancellationToken);
        return feature is null ? NotFound() : Ok(_mapper.Map<PropertyFeatureDto>(feature));
    }

    [HttpPost]
    [ProducesResponseType(typeof(PropertyFeatureDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Create a property feature")]
    [EndpointDescription("Adds a new property feature to the catalog.")]
    public async Task<ActionResult<PropertyFeatureDto>> Create([FromBody] CreatePropertyFeatureDto request, CancellationToken cancellationToken)
    {
        var feature = await _sender.Send(new CreatePropertyFeatureCommand { Name = request.Name, Icon = request.Icon }, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = feature.Id }, _mapper.Map<PropertyFeatureDto>(feature));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PropertyFeatureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Update a property feature")]
    [EndpointDescription("Updates the specified property feature.")]
    public async Task<ActionResult<PropertyFeatureDto>> Update(int id, [FromBody] UpdatePropertyFeatureDto request, CancellationToken cancellationToken)
    {
        var feature = await _sender.Send(new UpdatePropertyFeatureCommand { Id = id, Name = request.Name, Icon = request.Icon }, cancellationToken);
        return feature is null ? NotFound() : Ok(_mapper.Map<PropertyFeatureDto>(feature));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Delete a property feature")]
    [EndpointDescription("Deletes the specified property feature.")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeletePropertyFeatureCommand { Id = id }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
