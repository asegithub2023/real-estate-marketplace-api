using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Interfaces.Repositories;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Conversation?> GetByPropertyAndUsersAsync(int propertyId, int buyerId, int ownerId, CancellationToken cancellationToken = default);
    Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default);
    Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default);
}
