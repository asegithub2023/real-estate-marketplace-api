using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Messages.Queries;

public sealed class GetConversationMessagesQueryHandler : IRequestHandler<GetConversationMessagesQuery, IReadOnlyList<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;

    public GetConversationMessagesQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<IReadOnlyList<MessageDto>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetByConversationIdAsync(request.ConversationId, cancellationToken);
        return messages.Select(message => new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content,
            SentAt = message.SentAt
        }).ToList();
    }
}
