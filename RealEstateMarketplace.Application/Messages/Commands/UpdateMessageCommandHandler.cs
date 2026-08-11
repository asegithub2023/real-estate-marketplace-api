using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Messages.Commands;

public sealed class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand, Result<Message, MessageError>>
{
    private readonly IMessageRepository _messageRepository;

    public UpdateMessageCommandHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<Result<Message, MessageError>> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _messageRepository.GetByIdAsync(request.Id, cancellationToken);
        if (message is null)
        {
            return Result.Failure<Message, MessageError>(MessageError.NotFound(request.Id));
        }

        if (request.Content is not null)
        {
            message.Content = request.Content;
        }

        await _messageRepository.UpdateAsync(message, cancellationToken);

        return Result.Success<Message, MessageError>(message);
    }
}
