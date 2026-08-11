using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Messages.Queries;

public sealed class GetConversationMessagesQueryHandler : IRequestHandler<GetConversationMessagesQuery, IReadOnlyList<Message>>
{
    private readonly IMessageRepository _messageRepository;

    public GetConversationMessagesQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<IReadOnlyList<Message>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        return await _messageRepository.GetByConversationIdAsync(request.ConversationId, cancellationToken);
    }
}
