using MediatR;

namespace RealEstateMarketplace.Application.Properties.Commands;

public sealed class DeletePropertyCommand : IRequest<bool>
{
    public int Id { get; set; }
}
