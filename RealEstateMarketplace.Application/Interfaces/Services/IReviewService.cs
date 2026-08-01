using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Interfaces.Services;

public interface IReviewService
{
    Task<ReviewDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReviewDto>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
    Task<ReviewDto> CreateAsync(CreateReviewDto request, CancellationToken cancellationToken = default);
    Task<ReviewDto?> UpdateAsync(int id, UpdateReviewDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
