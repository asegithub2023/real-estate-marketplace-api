using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Application.Mapping;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Infrastructure.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;

    public MessageService(IMessageRepository messageRepository, IConversationRepository conversationRepository, IUserRepository userRepository)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<MessageDto>> GetByConversationIdAsync(int conversationId, CancellationToken cancellationToken = default)
    {
        var messages = await _messageRepository.GetByConversationIdAsync(conversationId, cancellationToken);
        return messages.Select(message => message.ToDto()).ToList();
    }

    public async Task<MessageDto> CreateAsync(CreateMessageDto request, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        var sender = await _userRepository.GetByIdAsync(request.SenderId, cancellationToken);
        if (conversation is null || sender is null)
        {
            throw new InvalidOperationException("Conversation or sender was not found.");
        }

        var message = new Message
        {
            ConversationId = request.ConversationId,
            SenderId = request.SenderId,
            Content = request.Content
        };

        await _messageRepository.AddAsync(message, cancellationToken);
        return message.ToDto();
    }

    public async Task<MessageDto?> UpdateAsync(int id, UpdateMessageDto request, CancellationToken cancellationToken = default)
    {
        var message = await _messageRepository.GetByIdAsync(id, cancellationToken);
        if (message is null)
        {
            return null;
        }

        if (request.Content is not null)
        {
            message.Content = request.Content;
        }

        await _messageRepository.UpdateAsync(message, cancellationToken);
        return message.ToDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var message = await _messageRepository.GetByIdAsync(id, cancellationToken);
        if (message is null)
        {
            return false;
        }

        await _messageRepository.DeleteAsync(message, cancellationToken);
        return true;
    }
}
