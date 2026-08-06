using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Conversations.Queries;

public sealed class GetConversationByIdQueryHandler : IRequestHandler<GetConversationByIdQuery, ConversationDto?>
{
    private readonly IConversationRepository _conversationRepository;

    public GetConversationByIdQueryHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<ConversationDto?> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (conversation is null)
        {
            return null;
        }

        return new ConversationDto
        {
            Id = conversation.Id,
            PropertyId = conversation.PropertyId,
            BuyerId = conversation.BuyerId,
            OwnerId = conversation.OwnerId,
            CreatedAt = conversation.CreatedAt
        };
    }
}
