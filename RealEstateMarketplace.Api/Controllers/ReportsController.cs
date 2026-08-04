using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Services;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("property/{propertyId:int}")]
    public async Task<ActionResult<IReadOnlyList<ReportDto>>> GetByPropertyId(int propertyId, CancellationToken cancellationToken)
    {
        var reports = await _reportService.GetByPropertyIdAsync(propertyId, cancellationToken);
        return Ok(reports);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReportDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var report = await _reportService.GetByIdAsync(id, cancellationToken);
        return report is null ? NotFound() : Ok(report);
    }

    [HttpPost]
    public async Task<ActionResult<ReportDto>> Create([FromBody] CreateReportDto request, CancellationToken cancellationToken)
    {
        var report = await _reportService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ReportDto>> Update(int id, [FromBody] UpdateReportDto request, CancellationToken cancellationToken)
    {
        var report = await _reportService.UpdateAsync(id, request, cancellationToken);
        return report is null ? NotFound() : Ok(report);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _reportService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
