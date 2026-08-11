using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Conversations.Commands;

public sealed class UpdateConversationCommandHandler : IRequestHandler<UpdateConversationCommand, Result<Conversation, ConversationError>>
{
    private readonly IConversationRepository _conversationRepository;

    public UpdateConversationCommandHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<Result<Conversation, ConversationError>> Handle(UpdateConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (conversation is null)
        {
            return Result.Failure<Conversation, ConversationError>(ConversationError.NotFound(request.Id));
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

        return Result.Success<Conversation, ConversationError>(conversation);
    }
}
