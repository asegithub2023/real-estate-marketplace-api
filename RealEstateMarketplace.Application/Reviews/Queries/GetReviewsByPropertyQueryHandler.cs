using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Reviews.Queries;

public sealed class GetReviewsByPropertyQueryHandler : IRequestHandler<GetReviewsByPropertyQuery, IReadOnlyList<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewsByPropertyQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IReadOnlyList<ReviewDto>> Handle(GetReviewsByPropertyQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);
        return reviews.Select(review => new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            UserId = review.UserId,
            PropertyId = review.PropertyId
        }).ToList();
    }
}
