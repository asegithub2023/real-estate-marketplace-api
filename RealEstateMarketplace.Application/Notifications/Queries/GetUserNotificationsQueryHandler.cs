using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Notifications.Queries;

public sealed class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, IReadOnlyList<Notification>>
{
    private readonly INotificationRepository _notificationRepository;

    public GetUserNotificationsQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IReadOnlyList<Notification>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        return await _notificationRepository.GetByUserIdAsync(request.UserId, cancellationToken);
    }
}
