using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Reports.Commands;

public sealed class ResolveReportCommand : IRequest<ReportDto?>
{
    public int Id { get; set; }
}
