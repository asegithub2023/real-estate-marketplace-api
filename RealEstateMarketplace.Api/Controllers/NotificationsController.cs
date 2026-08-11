using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Notifications.Commands;
using RealEstateMarketplace.Application.Notifications.Queries;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public NotificationController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        var notifications = await _sender.Send(new GetUserNotificationsQuery { UserId = userId }, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<NotificationDto>>(notifications));
    }

    [HttpPost]
    public async Task<ActionResult<NotificationDto>> Create([FromBody] CreateNotificationDto request, CancellationToken cancellationToken)
    {
        var notification = await _sender.Send(new CreateNotificationCommand
        {
            Title = request.Title,
            Message = request.Message,
            IsRead = request.IsRead,
            UserId = request.UserId
        }, cancellationToken);

        return CreatedAtAction(nameof(GetByUserId), new { userId = notification.UserId }, notification);
    }

    [HttpPut("{id:int}/read")]
    public async Task<ActionResult<NotificationDto>> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        var notification = await _sender.Send(new MarkNotificationAsReadCommand { Id = id }, cancellationToken);
        return notification is null ? NotFound() : Ok(_mapper.Map<NotificationDto>(notification));
    }
}
