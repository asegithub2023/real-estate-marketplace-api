using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Reviews.Commands;

public sealed class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, ReviewDto?>
{
    private readonly IReviewRepository _reviewRepository;

    public UpdateReviewCommandHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewDto?> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review is null)
        {
            return null;
        }

        if (request.Rating is not null)
        {
            review.Rating = request.Rating.Value;
        }

        if (request.Comment is not null)
        {
            review.Comment = request.Comment;
        }

        await _reviewRepository.UpdateAsync(review, cancellationToken);

        return new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            UserId = review.UserId,
            PropertyId = review.PropertyId
        };
    }
}
