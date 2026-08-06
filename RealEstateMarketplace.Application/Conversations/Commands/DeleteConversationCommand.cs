using MediatR;

namespace RealEstateMarketplace.Application.Conversations.Commands;

public sealed class DeleteConversationCommand : IRequest<bool>
{
    public int Id { get; set; }
}
