using MediatR;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Messages.Queries;

public sealed class GetMessageByIdQuery : IRequest<Message?>
{
    public int Id { get; set; }
}
