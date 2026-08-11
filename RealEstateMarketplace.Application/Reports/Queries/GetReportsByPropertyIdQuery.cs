using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reports.Queries;

public sealed class GetReportsByPropertyIdQuery : IRequest<IReadOnlyList<Report>>
{
    public int PropertyId { get; set; }
}
