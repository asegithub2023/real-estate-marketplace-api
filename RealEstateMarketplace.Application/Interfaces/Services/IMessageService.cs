using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Interfaces.Services;

public interface IMessageService
{
    Task<IReadOnlyList<MessageDto>> GetByConversationIdAsync(int conversationId, CancellationToken cancellationToken = default);
    Task<MessageDto> CreateAsync(CreateMessageDto request, CancellationToken cancellationToken = default);
    Task<MessageDto?> UpdateAsync(int id, UpdateMessageDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
