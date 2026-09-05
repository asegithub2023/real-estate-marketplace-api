using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Api.Security;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Reports.Commands;
using RealEstateMarketplace.Application.Reports.Queries;
using System.Security.Claims;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Tags("Reports")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
[ApiVersion("1.0")]
public class ReportsController : ControllerBase
{
    private readonly ISender _sender;

    public ReportsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    [ProducesResponseType(typeof(IReadOnlyList<ReportDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Get all reports")]
    [EndpointDescription("Returns every report on the platform. Admin only.")]
    public async Task<ActionResult<IReadOnlyList<ReportDto>>> GetAll(CancellationToken cancellationToken)
    {
        var reports = await _sender.Send(new GetAllReportsQuery(), cancellationToken);
        return Ok(reports);
    }

    [HttpGet("property/{propertyId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<ReportDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Get reports by property")]
    [EndpointDescription("Returns all reports associated with the specified property.")]
    public async Task<ActionResult<IReadOnlyList<ReportDto>>> GetByPropertyId(int propertyId, CancellationToken cancellationToken)
    {
        var reports = await _sender.Send(new GetReportsByPropertyIdQuery { PropertyId = propertyId }, cancellationToken);
        return Ok(reports);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get a report by ID")]
    [EndpointDescription("Returns the report matching the specified identifier.")]
    public async Task<ActionResult<ReportDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new GetReportByIdQuery { Id = id }, cancellationToken);
        return report is null ? NotFound() : Ok(report);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Create a report")]
    [EndpointDescription("Creates a new report for a property, filed by the current logged-in user.")]
    public async Task<ActionResult<ReportDto>> Create([FromBody] CreateReportDto request, CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(currentUserId, out var userId))
        {
            return Unauthorized();
        }

        var report = await _sender.Send(new CreateReportCommand
        {
            Reason = request.Reason,
            UserId = userId,
            PropertyId = request.PropertyId
        }, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Update a report")]
    [EndpointDescription("Updates an existing report's reason by ID.")]
    public async Task<ActionResult<ReportDto>> Update(int id, [FromBody] UpdateReportDto request, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new UpdateReportCommand
        {
            Id = id,
            Reason = request.Reason
        }, cancellationToken);

        return report is null ? NotFound() : Ok(report);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Policy = Policies.AdminOnly)]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Update a report's status")]
    [EndpointDescription("Marks a report as Reviewed or Dismissed. Admin only.")]
    public async Task<ActionResult<ReportDto>> UpdateStatus(int id, [FromBody] UpdateReportStatusDto request, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new ResolveReportCommand
        {
            Id = id,
            Status = request.Status
        }, cancellationToken);

        return report is null ? NotFound() : Ok(report);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Delete a report")]
    [EndpointDescription("Permanently deletes the specified report. Admin only.")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeleteReportCommand { Id = id }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
