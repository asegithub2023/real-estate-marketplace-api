using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Reports.Queries;

public sealed class GetAllReportsQuery : IRequest<IReadOnlyList<ReportDto>>
{
}
