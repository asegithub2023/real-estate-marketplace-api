using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Reviews.Queries;

public sealed class GetReviewByIdQuery : IRequest<ReviewDto?>
{
    public int Id { get; set; }
}
