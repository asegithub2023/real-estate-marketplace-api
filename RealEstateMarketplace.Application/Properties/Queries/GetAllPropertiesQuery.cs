using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Properties.Queries;

public sealed class GetAllPropertiesQuery : IRequest<IReadOnlyList<PropertyDto>>
{
}
