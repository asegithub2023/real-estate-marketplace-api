using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reviews.Queries;

public sealed class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, Review?>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewByIdQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<Review?> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        return await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}
