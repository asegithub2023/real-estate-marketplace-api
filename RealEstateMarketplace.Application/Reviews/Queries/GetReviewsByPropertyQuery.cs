using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Reviews.Queries;

public sealed class GetReviewsByPropertyQuery : IRequest<IReadOnlyList<ReviewDto>>
{
    public int PropertyId { get; set; }
}
