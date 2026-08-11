using AutoMapper;
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
public class MessageController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public MessageController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet("conversation/{conversationId:int}")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetByConversationId(int conversationId, CancellationToken cancellationToken)
    {
        var messages = await _sender.Send(new GetConversationMessagesQuery { ConversationId = conversationId }, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<MessageDto>>(messages));
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> Create([FromBody] CreateMessageDto request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateMessageCommand>(request);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetByConversationId), new { conversationId = result.Value!.ConversationId }, _mapper.Map<MessageDto>(result.Value))
            : BadRequest(result.Error!.Message);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MessageDto>> Update(int id, [FromBody] UpdateMessageDto request, CancellationToken cancellationToken)
    {
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
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _sender.Send(new DeleteMessageCommand { Id = id }, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
