using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Reports.Queries;

public sealed class GetReportByIdQuery : IRequest<ReportDto?>
{
    public int Id { get; set; }
}
