using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Properties.Queries;

public sealed class GetAllPropertiesQuery : IRequest<IReadOnlyList<Property>>
{
}
