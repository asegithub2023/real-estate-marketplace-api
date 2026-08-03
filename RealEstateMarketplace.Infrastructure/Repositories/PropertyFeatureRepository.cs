using Microsoft.EntityFrameworkCore;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;
using RealEstateMarketplace.Infrastructure.Persistence;

namespace RealEstateMarketplace.Infrastructure.Repositories;

public class PropertyFeatureRepository : IPropertyFeatureRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyFeatureRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PropertyFeature?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyFeatures.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<PropertyFeature>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PropertyFeatures.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<PropertyFeature?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyFeatures.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task AddAsync(PropertyFeature feature, CancellationToken cancellationToken = default)
    {
        await _context.PropertyFeatures.AddAsync(feature, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PropertyFeature feature, CancellationToken cancellationToken = default)
    {
        _context.PropertyFeatures.Update(feature);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var feature = await _context.PropertyFeatures.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (feature is null)
        {
            return;
        }

        _context.PropertyFeatures.Remove(feature);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
