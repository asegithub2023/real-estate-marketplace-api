using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;

namespace RealEstateMarketplace.Application.Messages.Commands;

public sealed class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, bool>
{
    private readonly IMessageRepository _messageRepository;

    public DeleteMessageCommandHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<bool> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.Id, cancellationToken);
        if (message is null)
        {
            return false;
        }

        await _messageRepository.DeleteAsync(message, cancellationToken);
        return true;
    }
}
