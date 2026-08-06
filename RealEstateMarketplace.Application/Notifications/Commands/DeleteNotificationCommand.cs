using MediatR;

namespace RealEstateMarketplace.Application.Notifications.Commands;

public sealed class DeleteNotificationCommand : IRequest<bool>
{
    public int Id { get; set; }
}
