using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Interfaces.Services;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<NotificationDto?> MarkAsReadAsync(int id, CancellationToken cancellationToken = default);
    Task<NotificationDto> CreateAsync(CreateNotificationDto request, CancellationToken cancellationToken = default);
}
