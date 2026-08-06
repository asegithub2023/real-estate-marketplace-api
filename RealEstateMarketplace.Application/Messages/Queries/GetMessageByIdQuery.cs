using MediatR;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Messages.Queries;

public sealed class GetMessageByIdQuery : IRequest<MessageDto?>
{
    public int Id { get; set; }
}
