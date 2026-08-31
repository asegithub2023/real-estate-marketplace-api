using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Mapping;

namespace RealEstateMarketplace.Application.Reports.Commands;

public sealed class UpdateReportCommandHandler : IRequestHandler<UpdateReportCommand, ReportDto?>
{
    private readonly IReportRepository _reportRepository;

    public UpdateReportCommandHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<ReportDto?> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _reportRepository.GetByIdAsync(request.Id, cancellationToken);
        if (report is null)
        {
            return null;
        }

        if (request.Reason is not null)
        {
            report.Reason = request.Reason;
        }

        await _reportRepository.UpdateAsync(report, cancellationToken);

        var updated = await _reportRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        return updated?.ToDto();
    }
}
