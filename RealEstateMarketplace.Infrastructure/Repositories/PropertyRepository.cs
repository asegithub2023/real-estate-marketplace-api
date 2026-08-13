using Microsoft.EntityFrameworkCore;
using RealEstateMarketplace.Application.DTOs;
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

    public async Task<(IReadOnlyList<Property> Items, int TotalCount)> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _context.Properties.AsNoTracking();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim();
            query = query.Where(x => 
                EF.Functions.ILike(x.Title, $"%{searchTerm}%") || 
                EF.Functions.ILike(x.Description, $"%{searchTerm}%"));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply ordering
        query = ApplyOrdering(query, request.OrderBy, request.Descending);

        // Apply pagination
        var items = await query
            .Include(x => x.Owner)
            .Include(x => x.Images)
            .Include(x => x.PropertyFeatures)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    private static IQueryable<Property> ApplyOrdering(IQueryable<Property> query, string? orderBy, bool descending)
    {
        var order = orderBy?.ToLower().Trim() ?? "id";

        return (order, descending) switch
        {
            ("title", true) => query.OrderByDescending(x => x.Title),
            ("title", false) => query.OrderBy(x => x.Title),
            ("price", true) => query.OrderByDescending(x => x.Price),
            ("price", false) => query.OrderBy(x => x.Price),
            ("city", true) => query.OrderByDescending(x => x.City),
            ("city", false) => query.OrderBy(x => x.City),
            ("area", true) => query.OrderByDescending(x => x.Area),
            ("area", false) => query.OrderBy(x => x.Area),
            ("bedrooms", true) => query.OrderByDescending(x => x.Bedrooms),
            ("bedrooms", false) => query.OrderBy(x => x.Bedrooms),
            (_, true) => query.OrderByDescending(x => x.Id),
            (_, false) => query.OrderBy(x => x.Id),
        };
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
