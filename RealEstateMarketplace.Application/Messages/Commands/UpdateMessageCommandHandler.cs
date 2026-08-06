using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Messages.Commands;

public sealed class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand, MessageDto?>
{
    private readonly IMessageRepository _messageRepository;

    public UpdateMessageCommandHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<MessageDto?> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.Id, cancellationToken);
        if (message is null)
        {
            return null;
        }

        if (request.Content is not null)
        {
            message.Content = request.Content;
        }

        await _messageRepository.UpdateAsync(message, cancellationToken);

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
