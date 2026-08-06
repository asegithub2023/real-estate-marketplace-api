using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Reviews.Commands;

public sealed class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, bool>
{
    private readonly IReviewRepository _reviewRepository;

    public DeleteReviewCommandHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
        if (review is null)
        {
            return false;
        }

        await _reviewRepository.DeleteAsync(review, cancellationToken);
        return true;
    }
}
