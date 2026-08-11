namespace RealEstateMarketplace.Infrastructure.Caching;

public static class CacheKeys
{
    public static string Property(int id) => $"Property_{id}";
    public const string PropertiesAll = "Properties_All";
}
