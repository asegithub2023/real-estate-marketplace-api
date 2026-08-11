using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reviews.Queries;

public sealed class GetReviewsByPropertyQuery : IRequest<IReadOnlyList<Review>>
{
    public int PropertyId { get; set; }
}
