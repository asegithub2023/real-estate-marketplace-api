using Microsoft.Extensions.Caching.Hybrid;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Application.Mapping;
using RealEstateMarketplace.Infrastructure.Caching;

namespace RealEstateMarketplace.Infrastructure.Services;

public class CachedPropertyService : ICachedPropertyService
{
    private readonly HybridCache _cache;
    private readonly IPropertyRepository _propertyRepository;

    public CachedPropertyService(HybridCache cache, IPropertyRepository propertyRepository)
    {
        _cache = cache;
        _propertyRepository = propertyRepository;
    }

    public async Task<PropertyDto?> GetPropertyAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.Property(id);
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(2)
        };
        var wasFactoryCalled = false;
        var result = await _cache.GetOrCreateAsync(cacheKey, async ct =>
        {
            wasFactoryCalled = true;
            var property = await _propertyRepository.GetByIdAsync(id, ct);
            return property is null ? null : property.ToDto();
        }, entryOptions, Array.Empty<string>(), cancellationToken);

        if (wasFactoryCalled)
            CacheMetrics.CacheMisses.Add(1);
        else
            CacheMetrics.CacheHits.Add(1);

        return result;
    }

    public async Task<IReadOnlyList<PropertyDto>> GetAllPropertiesAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.PropertiesAll;
        var entryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(2)
        };
        var wasFactoryCalledAll = false;
        var resultAll = await _cache.GetOrCreateAsync(CacheKeys.PropertiesAll, async ct =>
        {
            wasFactoryCalledAll = true;
            var properties = await _propertyRepository.GetAllAsync(ct);
            return properties.Select(property => property.ToDto()).ToList();
        }, entryOptions, Array.Empty<string>(), cancellationToken);

        if (wasFactoryCalledAll)
            CacheMetrics.CacheMisses.Add(1);
        else
            CacheMetrics.CacheHits.Add(1);

        return resultAll;
    }

    public async Task InvalidatePropertyCacheAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id.HasValue)
        {
            await _cache.RemoveAsync(CacheKeys.Property(id.Value), cancellationToken);
        }

        await _cache.RemoveAsync(CacheKeys.PropertiesAll, cancellationToken);
    }
}
