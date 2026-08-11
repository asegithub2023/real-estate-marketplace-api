using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reviews.Commands;

public sealed class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, Result<Review, ReviewError>>
{
    private readonly IReviewRepository _reviewRepository;

    public UpdateReviewCommandHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<Review, ReviewError>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review is null)
        {
            return Result.Failure<Review, ReviewError>(ReviewError.NotFound(request.Id));
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

        return Result.Success<Review, ReviewError>(review);
    }
}
