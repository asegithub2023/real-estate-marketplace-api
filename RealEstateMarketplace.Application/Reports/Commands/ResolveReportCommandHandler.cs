using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Reports.Commands;

public sealed class ResolveReportCommandHandler : IRequestHandler<ResolveReportCommand, ReportDto?>
{
    private readonly IReportRepository _reportRepository;

    public ResolveReportCommandHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<ReportDto?> Handle(ResolveReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _reportRepository.GetByIdAsync(request.Id, cancellationToken);
        if (report is null)
        {
            return null;
        }

        await _reportRepository.DeleteAsync(report, cancellationToken);

        return new ReportDto
        {
            Id = report.Id,
            Reason = report.Reason,
            UserId = report.UserId,
            PropertyId = report.PropertyId
        };
    }
}
