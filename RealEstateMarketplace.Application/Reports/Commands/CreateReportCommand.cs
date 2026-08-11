using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reports.Commands;

public sealed class CreateReportCommand : IRequest<Report>
{
    public required string Reason { get; set; }
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}
