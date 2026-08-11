using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.PropertyFeatures.Commands;
using RealEstateMarketplace.Application.PropertyFeatures.Queries;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<ActionResult<IReadOnlyList<PropertyFeatureDto>>> GetAll(CancellationToken cancellationToken)
    {
        var features = await _sender.Send(new GetAllPropertyFeaturesQuery(), cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<PropertyFeatureDto>>(features));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<PropertyFeatureDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var feature = await _sender.Send(new GetPropertyFeatureByIdQuery { Id = id }, cancellationToken);
        return feature is null ? NotFound() : Ok(_mapper.Map<PropertyFeatureDto>(feature));
    }

    [HttpPost]
    public async Task<ActionResult<PropertyFeatureDto>> Create([FromBody] CreatePropertyFeatureDto request, CancellationToken cancellationToken)
    {
        var feature = await _sender.Send(new CreatePropertyFeatureCommand { Name = request.Name, Icon = request.Icon }, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = feature.Id }, _mapper.Map<PropertyFeatureDto>(feature));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PropertyFeatureDto>> Update(int id, [FromBody] UpdatePropertyFeatureDto request, CancellationToken cancellationToken)
    {
        var feature = await _sender.Send(new UpdatePropertyFeatureCommand { Id = id, Name = request.Name, Icon = request.Icon }, cancellationToken);
        return feature is null ? NotFound() : Ok(_mapper.Map<PropertyFeatureDto>(feature));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeletePropertyFeatureCommand { Id = id }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
