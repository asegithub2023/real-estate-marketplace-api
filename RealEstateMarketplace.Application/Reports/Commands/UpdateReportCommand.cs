using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reports.Commands;

public sealed class UpdateReportCommand : IRequest<Report?>
{
    public int Id { get; set; }
    public string? Reason { get; set; }
}
