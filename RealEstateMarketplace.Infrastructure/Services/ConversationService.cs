using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Infrastructure.Services;

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPropertyRepository _propertyRepository;

    public ConversationService(IConversationRepository conversationRepository, IUserRepository userRepository, IPropertyRepository propertyRepository)
    {
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<ConversationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(id, cancellationToken);
        return conversation is null ? null : MapToDto(conversation);
    }

    public async Task<IReadOnlyList<ConversationDto>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var conversations = await _conversationRepository.GetByUserIdAsync(userId, cancellationToken);
        return conversations.Select(MapToDto).ToList();
    }

    public async Task<ConversationDto> CreateAsync(CreateConversationDto request, CancellationToken cancellationToken = default)
    {
        var buyer = await _userRepository.GetByIdAsync(request.BuyerId, cancellationToken);
        var owner = await _userRepository.GetByIdAsync(request.OwnerId, cancellationToken);
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (buyer is null || owner is null || property is null)
        {
            throw new InvalidOperationException("Buyer, owner, or property was not found.");
        }

        var conversation = new Conversation
        {
            PropertyId = request.PropertyId,
            BuyerId = request.BuyerId,
            OwnerId = request.OwnerId
        };

        await _conversationRepository.AddAsync(conversation, cancellationToken);
        return MapToDto(conversation);
    }

    public async Task<ConversationDto?> UpdateAsync(int id, UpdateConversationDto request, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(id, cancellationToken);
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
        return MapToDto(conversation);
    }

    private static ConversationDto MapToDto(Conversation conversation)
    {
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
