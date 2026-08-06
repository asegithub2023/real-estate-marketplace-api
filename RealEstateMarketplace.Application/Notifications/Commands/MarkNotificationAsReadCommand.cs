using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Notifications.Commands;

public sealed class MarkNotificationAsReadCommand : IRequest<NotificationDto?>
{
    public int Id { get; set; }
}
