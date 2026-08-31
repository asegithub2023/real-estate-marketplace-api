using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Reports.Queries;

public sealed class GetReportsByPropertyIdQuery : IRequest<IReadOnlyList<ReportDto>>
{
    public int PropertyId { get; set; }
}