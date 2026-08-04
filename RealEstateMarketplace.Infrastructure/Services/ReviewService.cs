using AutoMapper;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IMapper _mapper;

    public ReviewService(IReviewRepository reviewRepository, IUserRepository userRepository, IPropertyRepository propertyRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
        _mapper = mapper;
    }

    public async Task<ReviewDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await _reviewRepository.GetByIdAsync(id, cancellationToken);
        return review is null ? null : _mapper.Map<ReviewDto>(review);
    }

    public async Task<IReadOnlyList<ReviewDto>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        var reviews = await _reviewRepository.GetByPropertyIdAsync(propertyId, cancellationToken);
        return reviews.Select(review => _mapper.Map<ReviewDto>(review)).ToList();
    }

    public async Task<ReviewDto> CreateAsync(CreateReviewDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (user is null || property is null)
        {
            throw new InvalidOperationException("User or property was not found.");
        }

        var review = new Review
        {
            Rating = request.Rating,
            Comment = request.Comment,
            UserId = request.UserId,
            PropertyId = request.PropertyId
        };

        await _reviewRepository.AddAsync(review, cancellationToken);
        return _mapper.Map<ReviewDto>(review);
    }

    public async Task<ReviewDto?> UpdateAsync(int id, UpdateReviewDto request, CancellationToken cancellationToken = default)
    {
        var review = await _reviewRepository.GetByIdAsync(id, cancellationToken);
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
        return _mapper.Map<ReviewDto>(review);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await _reviewRepository.GetByIdAsync(id, cancellationToken);
        if (review is null)
        {
            return false;
        }

        await _reviewRepository.DeleteAsync(review, cancellationToken);
        return true;
    }
}
