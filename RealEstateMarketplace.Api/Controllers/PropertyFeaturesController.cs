using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Services;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PropertyFeaturesController : ControllerBase
{
    private readonly IPropertyFeatureService _propertyFeatureService;

    public PropertyFeaturesController(IPropertyFeatureService propertyFeatureService)
    {
        _propertyFeatureService = propertyFeatureService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<PropertyFeatureDto>>> GetAll(CancellationToken cancellationToken)
    {
        var features = await _propertyFeatureService.GetAllAsync(cancellationToken);
        return Ok(features);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<PropertyFeatureDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var feature = await _propertyFeatureService.GetByIdAsync(id, cancellationToken);
        return feature is null ? NotFound() : Ok(feature);
    }

    [HttpPost]
    public async Task<ActionResult<PropertyFeatureDto>> Create([FromBody] CreatePropertyFeatureDto request, CancellationToken cancellationToken)
    {
        var feature = await _propertyFeatureService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = feature.Id }, feature);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PropertyFeatureDto>> Update(int id, [FromBody] UpdatePropertyFeatureDto request, CancellationToken cancellationToken)
    {
        var feature = await _propertyFeatureService.UpdateAsync(id, request, cancellationToken);
        return feature is null ? NotFound() : Ok(feature);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _propertyFeatureService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
