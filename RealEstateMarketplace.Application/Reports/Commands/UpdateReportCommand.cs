using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Reports.Commands;

public sealed class UpdateReportCommand : IRequest<ReportDto?>
{
    public int Id { get; set; }
    public string? Reason { get; set; }
}