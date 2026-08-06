using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Notifications.Queries;

public sealed class GetUserNotificationsQuery : IRequest<IReadOnlyList<NotificationDto>>
{
    public int UserId { get; set; }
}
