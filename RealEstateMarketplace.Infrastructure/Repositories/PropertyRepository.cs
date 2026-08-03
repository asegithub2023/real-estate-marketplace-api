using Microsoft.EntityFrameworkCore;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;
using RealEstateMarketplace.Infrastructure.Persistence;

namespace RealEstateMarketplace.Infrastructure.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Property?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .AsNoTracking()
            .Include(x => x.Owner)
            .Include(x => x.Images)
            .Include(x => x.PropertyFeatures)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Property>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .AsNoTracking()
            .Include(x => x.Owner)
            .Include(x => x.Images)
            .Include(x => x.PropertyFeatures)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Property>> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .AsNoTracking()
            .Where(x => x.OwnerId == ownerId)
            .Include(x => x.Owner)
            .Include(x => x.Images)
            .Include(x => x.PropertyFeatures)
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Property property, CancellationToken cancellationToken = default)
    {
        await _context.Properties.AddAsync(property, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Property property, CancellationToken cancellationToken = default)
    {
        _context.Properties.Update(property);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Property property, CancellationToken cancellationToken = default)
    {
        _context.Properties.Remove(property);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
