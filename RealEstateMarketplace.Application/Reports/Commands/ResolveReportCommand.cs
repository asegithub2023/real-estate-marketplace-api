using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Domain.Enums;

namespace RealEstateMarketplace.Application.Reports.Commands;

// Sets a report's status (Reviewed/Dismissed). Named "Resolve" because this is the
// action an admin takes to close out a report - it no longer deletes the report.
public sealed class ResolveReportCommand : IRequest<ReportDto?>
{
    public int Id { get; set; }
    public ReportStatus Status { get; set; }
}
