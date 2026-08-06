using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Messages.Commands;

public sealed class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, Result<MessageDto, MessageError>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;

    public CreateMessageCommandHandler(
        IMessageRepository messageRepository,
        IConversationRepository conversationRepository,
        IUserRepository userRepository)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<MessageDto, MessageError>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        var sender = await _userRepository.GetByIdAsync(request.SenderId, cancellationToken);
        if (conversation is null || sender is null)
        {
            return Result.Failure<MessageDto, MessageError>(MessageError.ConversationOrSenderNotFound());
        }

        var message = new Message
        {
            ConversationId = request.ConversationId,
            SenderId = request.SenderId,
            Content = request.Content
        };

        await _messageRepository.AddAsync(message, cancellationToken);

        return Result.Success<MessageDto, MessageError>(new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content,
            SentAt = message.SentAt
        });
    }
}
