using MediatR;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Messages.Queries;

public sealed class GetMessageByIdQueryHandler : IRequestHandler<GetMessageByIdQuery, Message?>
{
    private readonly IMessageRepository _messageRepository;

    public GetMessageByIdQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<Message?> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
    {
        return await _messageRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}
