using Microsoft.EntityFrameworkCore;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;
using RealEstateMarketplace.Infrastructure.Persistence;

namespace RealEstateMarketplace.Infrastructure.Repositories;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly ApplicationDbContext _context;

    public FavoriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Favorite?> GetByUserAndPropertyAsync(int userId, int propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.Favorites
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.PropertyId == propertyId, cancellationToken);
    }

    public async Task<IReadOnlyList<Favorite>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Favorites
            .AsNoTracking()
            .Include(x => x.Property)
                .ThenInclude(p => p.Images)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Favorite favorite, CancellationToken cancellationToken = default)
    {
        await _context.Favorites.AddAsync(favorite, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int userId, int propertyId, CancellationToken cancellationToken = default)
    {
        var favorite = await _context.Favorites.FirstOrDefaultAsync(x => x.UserId == userId && x.PropertyId == propertyId, cancellationToken);
        if (favorite is null)
        {
            return;
        }

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync(cancellationToken);
    }
}