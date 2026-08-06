using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Properties.Queries;

public sealed class GetPropertiesByOwnerIdQuery : IRequest<IReadOnlyList<PropertyDto>>
{
    public int OwnerId { get; set; }
}
