using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Conversations.Commands;

public sealed class UpdateConversationCommandHandler : IRequestHandler<UpdateConversationCommand, ConversationDto?>
{
    private readonly IConversationRepository _conversationRepository;

    public UpdateConversationCommandHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<ConversationDto?> Handle(UpdateConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (conversation is null)
        {
            return null;
        }

        if (request.PropertyId is not null)
        {
            conversation.PropertyId = request.PropertyId.Value;
        }

        if (request.BuyerId is not null)
        {
            conversation.BuyerId = request.BuyerId.Value;
        }

        if (request.OwnerId is not null)
        {
            conversation.OwnerId = request.OwnerId.Value;
        }

        await _conversationRepository.UpdateAsync(conversation, cancellationToken);

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
