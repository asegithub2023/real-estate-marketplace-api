using Microsoft.EntityFrameworkCore;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;
using RealEstateMarketplace.Infrastructure.Persistence;

namespace RealEstateMarketplace.Infrastructure.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly ApplicationDbContext _context;

    public ConversationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private IQueryable<Conversation> Enriched() => _context.Conversations
        .AsNoTracking()
        .Include(c => c.Property).ThenInclude(p => p.Images)
        .Include(c => c.Buyer)
        .Include(c => c.Owner)
        .Include(c => c.Messages.OrderBy(m => m.SentAt));

    public async Task<Conversation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Enriched().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Conversation>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Enriched()
            .Where(x => x.BuyerId == userId || x.OwnerId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Conversation?> GetByPropertyAndUsersAsync(int propertyId, int buyerId, int ownerId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PropertyId == propertyId && x.BuyerId == buyerId && x.OwnerId == ownerId, cancellationToken);
    }

    public async Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        await _context.Conversations.AddAsync(conversation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {

        _context.Attach(conversation);
        _context.Entry(conversation).State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        _context.Conversations.Remove(conversation);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
