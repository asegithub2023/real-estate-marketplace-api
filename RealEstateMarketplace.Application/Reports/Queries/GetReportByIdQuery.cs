using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reports.Queries;

public sealed class GetReportByIdQuery : IRequest<Report?>
{
    public int Id { get; set; }
}
