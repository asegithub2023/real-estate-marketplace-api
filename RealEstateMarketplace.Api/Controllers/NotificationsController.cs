using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Services;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        var notifications = await _notificationService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(notifications);
    }

    [HttpPost]
    public async Task<ActionResult<NotificationDto>> Create([FromBody] CreateNotificationDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var notification = await _notificationService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByUserId), new { userId = notification.UserId }, notification);
    }

    [HttpPut("{id:int}/read")]
    public async Task<ActionResult<NotificationDto>> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        var notification = await _notificationService.MarkAsReadAsync(id, cancellationToken);
        return notification is null ? NotFound() : Ok(notification);
    }
}
