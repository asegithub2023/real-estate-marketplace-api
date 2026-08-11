using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Reports.Commands;
using RealEstateMarketplace.Application.Reports.Queries;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<ActionResult<IReadOnlyList<ReportDto>>> GetByPropertyId(int propertyId, CancellationToken cancellationToken)
    {
        var reports = await _sender.Send(new GetReportsByPropertyIdQuery { PropertyId = propertyId }, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<ReportDto>>(reports));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReportDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var report = await _sender.Send(new GetReportByIdQuery { Id = id }, cancellationToken);
        return report is null ? NotFound() : Ok(_mapper.Map<ReportDto>(report));
    }

    [HttpPost]
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
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeleteReportCommand { Id = id }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
