using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Domain.Enums;

namespace RealEstateMarketplace.Application.Reports.Commands;

public sealed class ResolveReportCommand : IRequest<ReportDto?>
{
    public int Id { get; set; }
    public ReportStatus Status { get; set; }
}
