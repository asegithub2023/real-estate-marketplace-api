using Microsoft.EntityFrameworkCore;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;
using RealEstateMarketplace.Infrastructure.Persistence;

namespace RealEstateMarketplace.Infrastructure.Repositories;

public class PropertyImageRepository : IPropertyImageRepository
{
    private readonly ApplicationDbContext _context;

    public PropertyImageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PropertyImage?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyImages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<PropertyImage>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyImages
            .AsNoTracking()
            .Where(x => x.PropertyId == propertyId)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(PropertyImage propertyImage, CancellationToken cancellationToken = default)
    {
        await _context.PropertyImages.AddAsync(propertyImage, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var image = await _context.PropertyImages.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (image is null)
        {
            return;
        }

        _context.PropertyImages.Remove(image);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        var images = await _context.PropertyImages.Where(x => x.PropertyId == propertyId).ToListAsync(cancellationToken);
        if (images.Count == 0)
        {
            return;
        }

        _context.PropertyImages.RemoveRange(images);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
