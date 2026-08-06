using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Conversations.Commands;

public sealed class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand, bool>
{
    private readonly IConversationRepository _conversationRepository;

    public DeleteConversationCommandHandler(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<bool> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (conversation is null)
        {
            return false;
        }

        await _conversationRepository.DeleteAsync(conversation, cancellationToken);
        return true;
    }
}
