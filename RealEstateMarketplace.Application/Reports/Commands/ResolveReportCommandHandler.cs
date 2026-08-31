using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Mapping;

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

        report.Status = request.Status;
        await _reportRepository.UpdateAsync(report, cancellationToken);

        var updated = await _reportRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        return updated?.ToDto();
    }
}