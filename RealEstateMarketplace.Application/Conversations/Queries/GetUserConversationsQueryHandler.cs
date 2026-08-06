using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Conversations.Queries;

public sealed class GetUserConversationsQueryHandler : IRequestHandler<GetUserConversationsQuery, IReadOnlyList<ConversationDto>>
{
    private readonly IConversationRepository _conversationRepository;

    public GetUserConversationsQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<IReadOnlyList<ConversationDto>> Handle(GetUserConversationsQuery request, CancellationToken cancellationToken)
    {
        var conversations = await _conversationRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        return conversations.Select(conversation => new ConversationDto
        {
            Id = conversation.Id,
            PropertyId = conversation.PropertyId,
            BuyerId = conversation.BuyerId,
            OwnerId = conversation.OwnerId,
            CreatedAt = conversation.CreatedAt
        }).ToList();
    }
}
