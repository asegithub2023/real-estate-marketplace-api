using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reports.Queries;

public sealed class GetReportsByPropertyIdQueryHandler : IRequestHandler<GetReportsByPropertyIdQuery, IReadOnlyList<Report>>
{
    private readonly IReportRepository _reportRepository;

    public GetReportsByPropertyIdQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<IReadOnlyList<Report>> Handle(GetReportsByPropertyIdQuery request, CancellationToken cancellationToken)
    {
        return await _reportRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);
    }
}
