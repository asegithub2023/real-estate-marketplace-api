using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Properties.Queries;

public sealed class GetPropertyByIdQuery : IRequest<Property?>
{
    public int Id { get; set; }
}
