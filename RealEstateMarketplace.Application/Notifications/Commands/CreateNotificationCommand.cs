using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Notifications.Commands;

public sealed class CreateNotificationCommand : IRequest<Notification>
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public int UserId { get; set; }
}
