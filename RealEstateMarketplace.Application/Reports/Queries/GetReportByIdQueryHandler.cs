using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Reports.Queries;

public sealed class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, ReportDto?>
{
    private readonly IReportRepository _reportRepository;

    public GetReportByIdQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<ReportDto?> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        var report = await _reportRepository.GetByIdAsync(request.Id, cancellationToken);
        return report is null ? null : new ReportDto
        {
            Id = report.Id,
            Reason = report.Reason,
            UserId = report.UserId,
            PropertyId = report.PropertyId
        };
    }
}
