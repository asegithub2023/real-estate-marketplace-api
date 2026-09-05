using AutoMapper;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using RealEstateMarketplace.Api.Hubs;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Conversations.Queries;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Mapping;
using RealEstateMarketplace.Application.Messages.Commands;
using RealEstateMarketplace.Application.Messages.Queries;
using Scalar.AspNetCore;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Tags("Messages")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
[ApiVersion("1.0")]
public class MessageController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    private readonly IHubContext<ChatHub> _hubContext;

    public MessageController(ISender sender, IMapper mapper, IHubContext<ChatHub> hubContext)
    {
        _sender = sender;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out userId);
    }

    private static bool IsParticipant(RealEstateMarketplace.Domain.Entities.Conversation conversation, int userId) =>
        conversation.BuyerId == userId || conversation.OwnerId == userId;

    [HttpGet("conversation/{conversationId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get messages in a conversation")]
    [EndpointDescription("Returns all messages belonging to the specified conversation, oldest first, and marks the other participant's messages as read. Only the buyer or owner on the conversation may access it.")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetByConversationId(int conversationId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var conversation = await _sender.Send(new GetConversationByIdQuery { Id = conversationId }, cancellationToken);
        if (conversation is null)
        {
            return NotFound();
        }

        if (!IsParticipant(conversation, userId))
        {
            return Forbid();
        }

        await _sender.Send(new MarkConversationMessagesAsReadCommand { ConversationId = conversationId, ReaderUserId = userId }, cancellationToken);

        var messages = await _sender.Send(new GetConversationMessagesQuery { ConversationId = conversationId }, cancellationToken);
        return Ok(messages.Select(m => m.ToDto()).ToList());
    }

    [HttpPost]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Send a message")]
    [EndpointDescription("Sends a new message in an existing conversation as the authenticated user and broadcasts it over SignalR. Only the buyer or owner on the conversation may send to it.")]
    public async Task<ActionResult<MessageDto>> Create([FromBody] CreateMessageDto request, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var conversation = await _sender.Send(new GetConversationByIdQuery { Id = request.ConversationId }, cancellationToken);
        if (conversation is null)
        {
            return NotFound();
        }

        if (!IsParticipant(conversation, userId))
        {
            return Forbid();
        }

        var command = _mapper.Map<CreateMessageCommand>(request);
        command.SenderId = userId;

        var result = await _sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error!.Message);
        }

        var dto = new MessageDto
        {
            Id = result.Value!.Id,
            ConversationId = result.Value.ConversationId,
            SenderId = result.Value.SenderId,
            SenderName = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
            Content = result.Value.Content,
            SentAt = result.Value.SentAt
        };

        await _hubContext.Clients
            .Group(ChatHub.ConversationGroup(dto.ConversationId))
            .SendAsync("ReceiveMessage", dto, cancellationToken);

        var updatedConversation = await _sender.Send(new GetConversationByIdQuery { Id = dto.ConversationId }, cancellationToken);
        if (updatedConversation is not null)
        {
            await _hubContext.Clients
                .Group(ChatHub.UserGroup(updatedConversation.BuyerId))
                .SendAsync("ConversationUpdated", updatedConversation.ToDto(updatedConversation.BuyerId), cancellationToken);

            await _hubContext.Clients
                .Group(ChatHub.UserGroup(updatedConversation.OwnerId))
                .SendAsync("ConversationUpdated", updatedConversation.ToDto(updatedConversation.OwnerId), cancellationToken);
        }

        return CreatedAtAction(nameof(GetByConversationId), new { conversationId = dto.ConversationId }, dto);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Update a message")]
    [EndpointDescription("Updates the text of an existing message by ID. Only the original sender may edit it.")]
    public async Task<ActionResult<MessageDto>> Update(int id, [FromBody] UpdateMessageDto request, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var existing = await _sender.Send(new GetMessageByIdQuery { Id = id }, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        if (existing.SenderId != userId)
        {
            return Forbid();
        }

        var command = _mapper.Map<UpdateMessageCommand>(request);
        command.Id = id;
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(_mapper.Map<MessageDto>(result.Value!))
            : result.Error!.Code == "message_not_found"
                ? NotFound(result.Error.Message)
                : BadRequest(result.Error.Message);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Delete a message")]
    [EndpointDescription("Deletes a message by its identifier. Only the original sender may delete it.")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var existing = await _sender.Send(new GetMessageByIdQuery { Id = id }, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        if (existing.SenderId != userId)
        {
            return Forbid();
        }

        var deleted = await _sender.Send(new DeleteMessageCommand { Id = id }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
