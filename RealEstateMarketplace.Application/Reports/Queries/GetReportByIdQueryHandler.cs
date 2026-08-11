using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reports.Queries;

public sealed class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, Report?>
{
    private readonly IReportRepository _reportRepository;

    public GetReportByIdQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<Report?> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        return await _reportRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}
