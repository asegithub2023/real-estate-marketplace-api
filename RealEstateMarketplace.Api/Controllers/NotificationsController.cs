using AutoMapper;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Notifications.Commands;
using RealEstateMarketplace.Application.Notifications.Queries;
using Scalar.AspNetCore;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Tags("Notifications")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
[ApiVersion("1.0")]
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
    [ProducesResponseType(typeof(IReadOnlyList<NotificationDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Get notifications for a user")]
    [EndpointDescription("Returns the list of notifications for the specified user.")]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        var notifications = await _sender.Send(new GetUserNotificationsQuery { UserId = userId }, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<NotificationDto>>(notifications));
    }

    [HttpPost]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Create a notification")]
    [EndpointDescription("Creates a new notification for a user.")]
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
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Mark notification as read")]
    [EndpointDescription("Marks the specified notification as read and returns the updated entity.")]
    public async Task<ActionResult<NotificationDto>> MarkAsRead(int id, CancellationToken cancellationToken)
    {
        var notification = await _sender.Send(new MarkNotificationAsReadCommand { Id = id }, cancellationToken);
        return notification is null ? NotFound() : Ok(_mapper.Map<NotificationDto>(notification));
    }
}
