using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Conversations.Queries;

public sealed class GetConversationByIdQueryHandler : IRequestHandler<GetConversationByIdQuery, Conversation?>
{
    private readonly IConversationRepository _conversationRepository;

    public GetConversationByIdQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<Conversation?> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        return await _conversationRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}
