using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Reports.Queries;

public sealed class GetAllReportsQueryHandler : IRequestHandler<GetAllReportsQuery, IReadOnlyList<ReportDto>>
{
    private readonly IReportRepository _reportRepository;

    public GetAllReportsQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<IReadOnlyList<ReportDto>> Handle(GetAllReportsQuery request, CancellationToken cancellationToken)
    {
        var reports = await _reportRepository.GetAllAsync(cancellationToken);
        return reports.Select(report => new ReportDto
        {
            Id = report.Id,
            Reason = report.Reason,
            UserId = report.UserId,
            PropertyId = report.PropertyId
        }).ToList();
    }
}
