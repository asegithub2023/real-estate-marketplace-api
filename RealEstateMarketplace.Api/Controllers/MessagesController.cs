using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Messages.Commands;
using RealEstateMarketplace.Application.Messages.Queries;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly ISender _sender;

    public MessagesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("conversation/{conversationId:int}")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetByConversationId(int conversationId, CancellationToken cancellationToken)
    {
        var messages = await _sender.Send(new GetConversationMessagesQuery { ConversationId = conversationId }, cancellationToken);
        return Ok(messages);
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> Create([FromBody] CreateMessageDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new CreateMessageCommand
        {
            ConversationId = request.ConversationId,
            SenderId = request.SenderId,
            Content = request.Content
        }, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetByConversationId), new { conversationId = result.Value!.ConversationId }, result.Value)
            : BadRequest(result.Error!.Message);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MessageDto>> Update(int id, [FromBody] UpdateMessageDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateMessageCommand
        {
            Id = id,
            Content = request.Content
        }, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value!)
            : result.Error!.Code == "message_not_found"
                ? NotFound(result.Error.Message)
                : BadRequest(result.Error.Message);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeleteMessageCommand { Id = id }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
