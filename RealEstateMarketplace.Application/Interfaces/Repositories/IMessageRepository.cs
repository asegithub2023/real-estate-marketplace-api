using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Interfaces.Repositories;

public interface IMessageRepository
{
    Task<IReadOnlyList<Message>> GetByConversationIdAsync(int conversationId, CancellationToken cancellationToken = default);
    Task<Message?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Message message, CancellationToken cancellationToken = default);
    Task UpdateAsync(Message message, CancellationToken cancellationToken = default);
    Task DeleteAsync(Message message, CancellationToken cancellationToken = default);

    /// Marks every unread message in the conversation NOT sent by <paramref name="readerUserId"/>
    /// as read. Returns the number of rows updated (0 means there was nothing new to mark).
    Task<int> MarkAsReadAsync(int conversationId, int readerUserId, CancellationToken cancellationToken = default);
}