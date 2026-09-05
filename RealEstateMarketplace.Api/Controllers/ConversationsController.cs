using AutoMapper;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Conversations.Commands;
using RealEstateMarketplace.Application.Conversations.Queries;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Mapping;
using Scalar.AspNetCore;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Tags("Conversations")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[Authorize]
[ApiVersion("1.0")]
public class ConversationsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public ConversationsController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    private bool TryGetCurrentUserId(out int userId)
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out userId);
    }

    [HttpGet("user/me")]
    [ProducesResponseType(typeof(IReadOnlyList<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Get conversations for the current user")]
    [EndpointDescription("Returns every conversation (as buyer or as owner) belonging to the authenticated user, newest first.")]
    public async Task<ActionResult<IReadOnlyList<ConversationDto>>> GetMyConversations(CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var conversations = await _sender.Send(new GetUserConversationsQuery { UserId = userId }, cancellationToken);

        return Ok(conversations.Select(c => c.ToDto(userId)).ToList());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [EndpointSummary("Get a conversation by ID")]
    [EndpointDescription("Returns the conversation details for the specified identifier. Only the buyer or owner on the conversation may access it.")]
    public async Task<ActionResult<ConversationDto>> GetById(int id, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var conversation = await _sender.Send(new GetConversationByIdQuery { Id = id }, cancellationToken);

        if (conversation is null)
        {
            return NotFound();
        }

        if (conversation.BuyerId != userId && conversation.OwnerId != userId)
        {
            return Forbid();
        }

        return Ok(conversation.ToDto(userId));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Start (or resume) a conversation about a property")]
    [EndpointDescription("Creates a new conversation between the authenticated user and the property's owner, or returns the existing one if it already exists.")]
    public async Task<ActionResult<ConversationDto>> Create([FromBody] CreateConversationDto request, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var command = _mapper.Map<CreateConversationCommand>(request);
        command.BuyerId = userId;

        var result = await _sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error!.Code == "cannot_contact_own_property"
                ? BadRequest(result.Error.Message)
                : NotFound(result.Error.Message);
        }

        var conversation = await _sender.Send(new GetConversationByIdQuery { Id = result.Value!.Id }, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = conversation!.Id }, conversation.ToDto(userId));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [EndpointSummary("Update a conversation")]
    [EndpointDescription("Updates an existing conversation by ID. Only the buyer or owner on the conversation may update it.")]
    public async Task<ActionResult<ConversationDto>> Update(int id, [FromBody] UpdateConversationDto request, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var existing = await _sender.Send(new GetConversationByIdQuery { Id = id }, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        if (existing.BuyerId != userId && existing.OwnerId != userId)
        {
            return Forbid();
        }

        var command = _mapper.Map<UpdateConversationCommand>(request);
        command.Id = id;
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Ok(_mapper.Map<ConversationDto>(result.Value!))
            : result.Error!.Code == "conversation_not_found"
                ? NotFound(result.Error.Message)
                : BadRequest(result.Error.Message);
    }
}
