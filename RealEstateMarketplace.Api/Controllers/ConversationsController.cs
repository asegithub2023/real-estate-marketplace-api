using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Conversations.Commands;
using RealEstateMarketplace.Application.Conversations.Queries;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConversationsController : ControllerBase
{
    private readonly ISender _sender;

    public ConversationsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IReadOnlyList<ConversationDto>>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        var conversations = await _sender.Send(new GetUserConversationsQuery { UserId = userId }, cancellationToken);
        return Ok(conversations);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ConversationDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var conversation = await _sender.Send(new GetConversationByIdQuery { Id = id }, cancellationToken);
        return conversation is null ? NotFound() : Ok(conversation);
    }

    [HttpPost]
    public async Task<ActionResult<ConversationDto>> Create([FromBody] CreateConversationDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateConversationCommand
        {
            PropertyId = request.PropertyId,
            BuyerId = request.BuyerId,
            OwnerId = request.OwnerId
        }, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : BadRequest(result.Error!.Message);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ConversationDto>> Update(int id, [FromBody] UpdateConversationDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateConversationCommand
        {
            Id = id,
            PropertyId = request.PropertyId,
            BuyerId = request.BuyerId,
            OwnerId = request.OwnerId
        }, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value!)
            : result.Error!.Code == "conversation_not_found"
                ? NotFound(result.Error.Message)
                : BadRequest(result.Error.Message);
    }
}
