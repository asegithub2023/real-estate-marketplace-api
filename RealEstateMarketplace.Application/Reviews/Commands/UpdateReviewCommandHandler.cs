using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Reviews.Commands;

public sealed class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Result<ReviewDto, ReviewError>>
{
    private readonly IReviewRepository _reviewRepository;

    public UpdateReviewCommandHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<ReviewDto, ReviewError>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review is null)
        {
            return Result.Failure<ReviewDto, ReviewError>(ReviewError.NotFound(request.Id));
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

        return Result.Success<ReviewDto, ReviewError>(new ReviewDto
        {
            Id = review.Id,
            Rating = review.Rating,
            Comment = review.Comment,
            UserId = review.UserId,
            PropertyId = review.PropertyId
        });
    }
}
