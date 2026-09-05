using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Mapping;

namespace RealEstateMarketplace.Application.Reports.Queries;

public sealed class GetReportsByPropertyIdQueryHandler : IRequestHandler<GetReportsByPropertyIdQuery, IReadOnlyList<ReportDto>>
{
    private readonly IReportRepository _reportRepository;

    public GetReportsByPropertyIdQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<IReadOnlyList<ReportDto>> Handle(GetReportsByPropertyIdQuery request, CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);
        return reports.Select(report => report.ToDto()).ToList();
    }
}
