using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Notifications.Commands;

public sealed class CreateNotificationCommand : IRequest<NotificationDto>
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public int UserId { get; set; }
}
