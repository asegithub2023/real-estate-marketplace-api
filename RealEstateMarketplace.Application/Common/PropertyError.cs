namespace RealEstateMarketplace.Application.Common;

public sealed record PropertyError(string Code, string Message)
{
    public static PropertyError NotFound(int id) =>
        new("property_not_found", $"Property '{id}' was not found.");

    public static PropertyError OwnerNotFound(int ownerId) =>
        new("owner_not_found", $"Owner '{ownerId}' was not found.");
}
