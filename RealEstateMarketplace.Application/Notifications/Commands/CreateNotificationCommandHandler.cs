using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Notifications.Commands;

public sealed class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, NotificationDto>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;

    public CreateNotificationCommandHandler(
        INotificationRepository notificationRepository,
        IUserRepository userRepository)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
    }

    public async Task<NotificationDto> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
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
