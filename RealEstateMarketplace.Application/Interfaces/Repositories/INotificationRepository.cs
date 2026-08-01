using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<IReadOnlyList<Notification>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Notification?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task DeleteAsync(Notification notification, CancellationToken cancellationToken = default);
}
