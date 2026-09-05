using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Conversations.Commands;

public sealed class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, Result<Conversation, ConversationError>>
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

    public async Task<Result<Conversation, ConversationError>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        var buyer = await _userRepository.GetByIdAsync(request.BuyerId, cancellationToken);

        if (property is null || buyer is null)
        {
            return Result.Failure<Conversation, ConversationError>(ConversationError.UserOrPropertyNotFound());
        }

        var ownerId = property.OwnerId;

        if (ownerId == request.BuyerId)
        {
            return Result.Failure<Conversation, ConversationError>(ConversationError.CannotContactOwnProperty());
        }

        var existing = await _conversationRepository.GetByPropertyAndUsersAsync(
            request.PropertyId, request.BuyerId, ownerId, cancellationToken);

        if (existing is not null)
        {
            return Result.Success<Conversation, ConversationError>(existing);
        }

        var conversation = new Conversation
        {
            PropertyId = request.PropertyId,
            BuyerId = request.BuyerId,
            OwnerId = ownerId
        };

        await _conversationRepository.AddAsync(conversation, cancellationToken);

        return Result.Success<Conversation, ConversationError>(conversation);
    }
}
