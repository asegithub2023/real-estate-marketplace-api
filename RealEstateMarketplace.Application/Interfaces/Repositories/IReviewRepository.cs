using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Interfaces.Repositories;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Review>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Review>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
    Task DeleteAsync(Review review, CancellationToken cancellationToken = default);
}
