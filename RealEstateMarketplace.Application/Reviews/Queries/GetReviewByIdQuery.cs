using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reviews.Queries;

public sealed class GetReviewByIdQuery : IRequest<Review?>
{
    public int Id { get; set; }
}
