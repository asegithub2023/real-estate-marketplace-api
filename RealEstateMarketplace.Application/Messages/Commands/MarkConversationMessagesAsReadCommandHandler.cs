using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Messages.Commands;

public sealed class MarkConversationMessagesAsReadCommandHandler : IRequestHandler<MarkConversationMessagesAsReadCommand, int>
{
    private readonly IMessageRepository _messageRepository;

    public MarkConversationMessagesAsReadCommandHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public Task<int> Handle(MarkConversationMessagesAsReadCommand request, CancellationToken cancellationToken)
    {
        return _messageRepository.MarkAsReadAsync(request.ConversationId, request.ReaderUserId, cancellationToken);
    }
}
