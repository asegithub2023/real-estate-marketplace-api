using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Reports.Commands;

public sealed class CreateReportCommand : IRequest<ReportDto>
{
    public required string Reason { get; set; }
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}