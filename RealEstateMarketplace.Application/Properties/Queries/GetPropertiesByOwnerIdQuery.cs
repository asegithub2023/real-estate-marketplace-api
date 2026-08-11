using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Properties.Queries;

public sealed class GetPropertiesByOwnerIdQuery : IRequest<IReadOnlyList<Property>>
{
    public int OwnerId { get; set; }
}
