using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reviews.Queries;

public sealed class GetReviewsByPropertyQueryHandler : IRequestHandler<GetReviewsByPropertyQuery, IReadOnlyList<Review>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewsByPropertyQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IReadOnlyList<Review>> Handle(GetReviewsByPropertyQuery request, CancellationToken cancellationToken)
    {
        return await _reviewRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);
    }
}
