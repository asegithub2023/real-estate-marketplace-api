using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reviews.Commands;

public sealed class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<ReviewDto, ReviewError>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPropertyRepository _propertyRepository;

    public CreateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        IPropertyRepository propertyRepository)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<ReviewDto, ReviewError>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);

        if (user is null || property is null)
        {
            return Result.Failure<ReviewDto, ReviewError>(ReviewError.UserOrPropertyNotFound());
        }

        var review = new Review
        {
            Rating = request.Rating,
            Comment = request.Comment,
            UserId = request.UserId,
            PropertyId = request.PropertyId
        };

        await _reviewRepository.AddAsync(review, cancellationToken);

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
