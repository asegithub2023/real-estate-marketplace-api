using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Interfaces.Repositories;

public interface IReportRepository
{
    Task<Report?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    // Same as GetByIdAsync but eager-loads User/Property for display (title, reporter name).
    // Kept separate from GetByIdAsync so entity mutations (update/delete) never carry the
    // extra navigation graph into the change tracker.
    Task<Report?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Report>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Report>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Report>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task AddAsync(Report report, CancellationToken cancellationToken = default);
    Task UpdateAsync(Report report, CancellationToken cancellationToken = default);
    Task DeleteAsync(Report report, CancellationToken cancellationToken = default);
}