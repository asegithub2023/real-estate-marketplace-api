using MediatR;
using RealEstateMarketplace.Application.Common;

namespace RealEstateMarketplace.Application.Properties.Commands;

public sealed class DeletePropertyCommand : IRequest<Result<bool, PropertyError>>
{
    public int Id { get; set; }
}
