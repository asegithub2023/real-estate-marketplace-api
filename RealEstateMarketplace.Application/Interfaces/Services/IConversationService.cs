using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Interfaces.Services;

public interface IConversationService
{
    Task<ConversationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConversationDto>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<ConversationDto> CreateAsync(CreateConversationDto request, CancellationToken cancellationToken = default);
    Task<ConversationDto?> UpdateAsync(int id, UpdateConversationDto request, CancellationToken cancellationToken = default);
}
