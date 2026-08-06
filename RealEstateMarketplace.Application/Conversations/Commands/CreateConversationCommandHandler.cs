using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Conversations.Commands;

public sealed class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, Result<ConversationDto, ConversationError>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPropertyRepository _propertyRepository;

    public CreateConversationCommandHandler(
        IConversationRepository conversationRepository,
        IUserRepository userRepository,
        IPropertyRepository propertyRepository)
    {
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<ConversationDto, ConversationError>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var buyer = await _userRepository.GetByIdAsync(request.BuyerId, cancellationToken);
        var owner = await _userRepository.GetByIdAsync(request.OwnerId, cancellationToken);
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (buyer is null || owner is null || property is null)
        {
            return Result.Failure<ConversationDto, ConversationError>(ConversationError.UserOrPropertyNotFound());
        }

        var conversation = new Conversation
        {
            PropertyId = request.PropertyId,
            BuyerId = request.BuyerId,
            OwnerId = request.OwnerId
        };

        await _conversationRepository.AddAsync(conversation, cancellationToken);

        return Result.Success<ConversationDto, ConversationError>(new ConversationDto
        {
            Id = conversation.Id,
            PropertyId = conversation.PropertyId,
            BuyerId = conversation.BuyerId,
            OwnerId = conversation.OwnerId,
            CreatedAt = conversation.CreatedAt
        });
    }
}
