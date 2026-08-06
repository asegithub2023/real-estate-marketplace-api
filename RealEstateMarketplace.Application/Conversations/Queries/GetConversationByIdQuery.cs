using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Conversations.Queries;

public sealed class GetConversationByIdQuery : IRequest<ConversationDto?>
{
    public int Id { get; set; }
}
