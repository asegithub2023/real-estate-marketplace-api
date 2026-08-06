using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Reviews.Commands;

public sealed class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Result<bool, ReviewError>>
{
    private readonly IReviewRepository _reviewRepository;

    public DeleteReviewCommandHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Result<bool, ReviewError>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review is null)
        {
            return Result.Failure<bool, ReviewError>(ReviewError.NotFound(request.Id));
        }

        await _reviewRepository.DeleteAsync(review, cancellationToken);
        return Result.Success<bool, ReviewError>(true);
    }
}
