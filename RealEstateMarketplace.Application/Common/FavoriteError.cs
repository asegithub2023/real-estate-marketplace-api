namespace RealEstateMarketplace.Application.Common;

public sealed record FavoriteError(string Code, string Message)
{
    public static FavoriteError AlreadyFavorited(int propertyId) =>
        new("favorite_already_exists", $"Property '{propertyId}' is already in favorites.");

    public static FavoriteError NotFound(int propertyId) =>
        new("favorite_not_found", $"Favorite for property '{propertyId}' was not found.");

    public static FavoriteError UserOrPropertyNotFound() =>
        new("user_or_property_not_found", "User or property was not found.");
}
