using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Notifications.Commands;

public sealed class UpdateNotificationCommand : IRequest<NotificationDto?>
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public bool? IsRead { get; set; }
}
