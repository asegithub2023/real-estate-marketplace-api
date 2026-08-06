using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Properties.Queries;

public sealed class GetPropertyByIdQuery : IRequest<PropertyDto?>
{
    public int Id { get; set; }
}
