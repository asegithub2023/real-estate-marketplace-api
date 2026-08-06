using MediatR;

namespace RealEstateMarketplace.Application.Reports.Commands;

public sealed class DeleteReportCommand : IRequest<bool>
{
    public int Id { get; set; }
}
