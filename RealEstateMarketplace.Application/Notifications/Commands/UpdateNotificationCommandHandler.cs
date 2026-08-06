using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Notifications.Commands;

public sealed class UpdateNotificationCommandHandler : IRequestHandler<UpdateNotificationCommand, NotificationDto?>
{
    private readonly INotificationRepository _notificationRepository;

    public UpdateNotificationCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<NotificationDto?> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (notification is null)
        {
            return null;
        }

        if (request.Title is not null)
        {
            notification.Title = request.Title;
        }

        if (request.Message is not null)
        {
            notification.Message = request.Message;
        }

        if (request.IsRead is not null)
        {
            notification.IsRead = request.IsRead.Value;
        }

        await _notificationRepository.UpdateAsync(notification, cancellationToken);

        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            UserId = notification.UserId
        };
    }
}
