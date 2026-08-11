using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Conversations.Queries;

public sealed class GetUserConversationsQueryHandler : IRequestHandler<GetUserConversationsQuery, IReadOnlyList<Conversation>>
{
    private readonly IConversationRepository _conversationRepository;

    public GetUserConversationsQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<IReadOnlyList<Conversation>> Handle(GetUserConversationsQuery request, CancellationToken cancellationToken)
    {
        return await _conversationRepository.GetByUserIdAsync(request.UserId, cancellationToken);
    }
}
