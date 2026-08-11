using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Notifications.Commands;

public sealed class MarkNotificationAsReadCommand : IRequest<Notification?>
{
    public int Id { get; set; }
}
