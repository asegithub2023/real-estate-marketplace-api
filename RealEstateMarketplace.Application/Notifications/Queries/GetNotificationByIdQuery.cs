using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Notifications.Queries;

public sealed class GetNotificationByIdQuery : IRequest<NotificationDto?>
{
    public int Id { get; set; }
}
