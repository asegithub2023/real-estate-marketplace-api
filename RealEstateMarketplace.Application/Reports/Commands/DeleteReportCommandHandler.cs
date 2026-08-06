using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Reports.Commands;

public sealed class DeleteReportCommandHandler : IRequestHandler<DeleteReportCommand, bool>
{
    private readonly IReportRepository _reportRepository;

    public DeleteReportCommandHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<bool> Handle(DeleteReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _reportRepository.GetByIdAsync(request.Id, cancellationToken);
        if (report is null)
        {
            return false;
        }

        await _reportRepository.DeleteAsync(report, cancellationToken);
        return true;
    }
}
