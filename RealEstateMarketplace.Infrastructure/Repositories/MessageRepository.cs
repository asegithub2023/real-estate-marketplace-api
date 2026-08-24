using Microsoft.EntityFrameworkCore;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;
using RealEstateMarketplace.Infrastructure.Persistence;

namespace RealEstateMarketplace.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Message>> GetByConversationIdAsync(int conversationId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .AsNoTracking()
            .Include(x => x.Sender)
            .Where(x => x.ConversationId == conversationId)
            .OrderBy(x => x.SentAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Message?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Messages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Message message, CancellationToken cancellationToken = default)
    {
        _context.Messages.Update(message);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Message message, CancellationToken cancellationToken = default)
    {
        _context.Messages.Remove(message);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<int> MarkAsReadAsync(int conversationId, int readerUserId, CancellationToken cancellationToken = default)
    {
        return _context.Messages
            .Where(m => m.ConversationId == conversationId && m.SenderId != readerUserId && !m.IsRead)
            .ExecuteUpdateAsync(setters => setters.SetProperty(m => m.IsRead, true), cancellationToken);
    }
}