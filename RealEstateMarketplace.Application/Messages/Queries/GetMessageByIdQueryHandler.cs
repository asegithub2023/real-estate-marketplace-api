using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Messages.Queries;

public sealed class GetMessageByIdQueryHandler : IRequestHandler<GetMessageByIdQuery, MessageDto?>
{
    private readonly IMessageRepository _messageRepository;

    public GetMessageByIdQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<MessageDto?> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.Id, cancellationToken);
        if (message is null)
        {
            return null;
        }

        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content,
            SentAt = message.SentAt
        };
    }
}
