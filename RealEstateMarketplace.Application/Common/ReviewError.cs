namespace RealEstateMarketplace.Application.Common;

public sealed record ReviewError(string Code, string Message)
{
    public static ReviewError NotFound(int id) =>
        new("review_not_found", $"Review '{id}' was not found.");

    public static ReviewError AlreadyReviewed(int propertyId) =>
        new("review_already_exists", $"You have already reviewed property '{propertyId}'.");

    public static ReviewError UserOrPropertyNotFound() =>
        new("user_or_property_not_found", "User or property was not found.");
}
