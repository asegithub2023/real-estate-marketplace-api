using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Application.Mapping;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;

    public NotificationService(INotificationRepository notificationRepository, IUserRepository userRepository)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<NotificationDto>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetByUserIdAsync(userId, cancellationToken);
        return notifications.Select(notification => notification.ToDto()).ToList();
    }

    public async Task<NotificationDto?> MarkAsReadAsync(int id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (notification is null)
        {
            return null;
        }

        notification.IsRead = true;
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
        return notification.ToDto();
    }

    public async Task<NotificationDto> CreateAsync(CreateNotificationDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            throw new InvalidOperationException("User was not found.");
        }

        var notification = new Notification
        {
            Title = request.Title,
            Message = request.Message,
            IsRead = request.IsRead,
            UserId = request.UserId
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
        return notification.ToDto();
    }
}
