using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Services;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("conversation/{conversationId:int}")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetByConversationId(int conversationId, CancellationToken cancellationToken)
    {
        var messages = await _messageService.GetByConversationIdAsync(conversationId, cancellationToken);
        return Ok(messages);
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> Create([FromBody] CreateMessageDto request, CancellationToken cancellationToken)
    {
        var message = await _messageService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByConversationId), new { conversationId = message.ConversationId }, message);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MessageDto>> Update(int id, [FromBody] UpdateMessageDto request, CancellationToken cancellationToken)
    {
        var message = await _messageService.UpdateAsync(id, request, cancellationToken);
        return message is null ? NotFound() : Ok(message);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _messageService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
