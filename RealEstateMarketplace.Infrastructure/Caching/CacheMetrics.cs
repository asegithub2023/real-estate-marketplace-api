using System.Diagnostics.Metrics;

namespace RealEstateMarketplace.Infrastructure.Caching;

public static class CacheMetrics
{
    public static readonly Meter Meter = new("realestate-api");

    public static readonly Counter<long> CacheHits =
        Meter.CreateCounter<long>("realestate.cache.hits", description: "Property cache hit count");

    public static readonly Counter<long> CacheMisses =
        Meter.CreateCounter<long>("realestate.cache.misses", description: "Property cache miss count");
}
