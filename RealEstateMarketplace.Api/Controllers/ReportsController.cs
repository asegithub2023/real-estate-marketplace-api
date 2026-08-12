using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Reports.Commands;
using RealEstateMarketplace.Application.Reports.Queries;
using Scalar.AspNetCore;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Tags("Reports")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public ReportsController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet("property/{propertyId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<ReportDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Get reports by property")]
    [EndpointDescription("Returns all reports associated with the specified property.")]
    public async Task<ActionResult<IReadOnlyList<ReportDto>>> GetByPropertyId(int propertyId, CancellationToken cancellationToken)
    {
        var reports = await _sender.Send(new GetReportsByPropertyIdQuery { PropertyId = propertyId }, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<ReportDto>>(reports));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get a report by ID")]
    [EndpointDescription("Returns the report matching the specified identifier.")]
    public async Task<ActionResult<ReportDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new GetReportByIdQuery { Id = id }, cancellationToken);
        return report is null ? NotFound() : Ok(_mapper.Map<ReportDto>(report));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Create a report")]
    [EndpointDescription("Creates a new report for a property.")]
    public async Task<ActionResult<ReportDto>> Create([FromBody] CreateReportDto request, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new CreateReportCommand
        {
            Reason = request.Reason,
            UserId = request.UserId,
            PropertyId = request.PropertyId
        }, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Update a report")]
    [EndpointDescription("Updates an existing report by ID.")]
    public async Task<ActionResult<ReportDto>> Update(int id, [FromBody] UpdateReportDto request, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new UpdateReportCommand
        {
            Id = id,
            Reason = request.Reason
        }, cancellationToken);

        return report is null ? NotFound() : Ok(_mapper.Map<ReportDto>(report));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Delete a report")]
    [EndpointDescription("Deletes the specified report.")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeleteReportCommand { Id = id }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
