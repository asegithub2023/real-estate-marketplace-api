using MediatR;

namespace RealEstateMarketplace.Application.Messages.Commands;

public sealed class DeleteMessageCommand : IRequest<bool>
{
    public int Id { get; set; }
}
