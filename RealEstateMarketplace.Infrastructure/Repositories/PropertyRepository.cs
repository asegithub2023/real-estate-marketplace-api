using Microsoft.EntityFrameworkCore;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;
using RealEstateMarketplace.Domain.Enums;
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

    public async Task<Property?> GetTrackedByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
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

    // Search: title, description, address, city, or a matching Property Type name
    // (e.g. searching "Villa" or "Apartment" returns properties of that type).
    if (!string.IsNullOrWhiteSpace(request.Search))
    {
        var searchTerm = request.Search.Trim();
        var matchedPropertyType = Enum.TryParse<PropertyType>(searchTerm, true, out var parsedType)
            ? parsedType
            : (PropertyType?)null;

        query = query.Where(x =>
            EF.Functions.ILike(x.Title, $"%{searchTerm}%") ||
            EF.Functions.ILike(x.Description, $"%{searchTerm}%") ||
            EF.Functions.ILike(x.Address, $"%{searchTerm}%") ||
            EF.Functions.ILike(x.City, $"%{searchTerm}%") ||
            (matchedPropertyType != null && x.PropertyType == matchedPropertyType));
    }

    // Filters
    if (request.MinPrice.HasValue)
        query = query.Where(x => x.Price >= request.MinPrice.Value);

    if (request.MaxPrice.HasValue)
        query = query.Where(x => x.Price <= request.MaxPrice.Value);

    if (request.MinBedrooms.HasValue)
        query = query.Where(x => x.Bedrooms >= request.MinBedrooms.Value);

    if (request.MinBathrooms.HasValue)
        query = query.Where(x => x.Bathrooms >= request.MinBathrooms.Value);

    if (!string.IsNullOrWhiteSpace(request.City))
        query = query.Where(x => EF.Functions.ILike(x.City, $"%{request.City.Trim()}%"));

    if (request.PropertyType.HasValue)
        query = query.Where(x => x.PropertyType == request.PropertyType.Value);

    if (request.ListingType.HasValue)
        query = query.Where(x => x.ListingType == request.ListingType.Value);

    // Get total count before pagination
    var totalCount = await query.CountAsync(cancellationToken);

    // Apply sorting
    query = ApplySorting(query, request);

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

   private static IQueryable<Property> ApplySorting(IQueryable<Property> query, PagedRequest request)
{
    return request.SortBy?.ToLower().Trim() switch
    {
        "newest" => query.OrderByDescending(x => x.Id),
        "priceasc" => query.OrderBy(x => x.Price),
        "pricedesc" => query.OrderByDescending(x => x.Price),
        "trending" => query.OrderByDescending(x => x.Favorites.Count),
        _ => ApplyOrdering(query, request.OrderBy, request.Descending)
    };
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