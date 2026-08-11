using AutoMapper;
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
    private readonly IMapper _mapper;

    public ConversationsController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet("user/{userId:int}")]
    public async Task<ActionResult<IReadOnlyList<ConversationDto>>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        var conversations = await _sender.Send(new GetUserConversationsQuery { UserId = userId }, cancellationToken);
        return Ok(_mapper.Map<IReadOnlyList<ConversationDto>>(conversations));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ConversationDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var conversation = await _sender.Send(new GetConversationByIdQuery { Id = id }, cancellationToken);
        return conversation is null ? NotFound() : Ok(_mapper.Map<ConversationDto>(conversation));
    }

    [HttpPost]
    public async Task<ActionResult<ConversationDto>> Create([FromBody] CreateConversationDto request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateConversationCommand>(request);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, _mapper.Map<ConversationDto>(result.Value))
            : BadRequest(result.Error!.Message);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ConversationDto>> Update(int id, [FromBody] UpdateConversationDto request, CancellationToken cancellationToken)
    {
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
