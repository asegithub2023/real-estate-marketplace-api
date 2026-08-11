using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Notifications.Queries;

public sealed class GetUserNotificationsQuery : IRequest<IReadOnlyList<Notification>>
{
    public int UserId { get; set; }
}
