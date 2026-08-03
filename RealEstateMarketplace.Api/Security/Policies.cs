namespace RealEstateMarketplace.Api.Security;

public static class Policies
{
    public const string AdminOnly = "AdminOnly";
    public const string OwnerOrAdmin = "OwnerOrAdmin";
    public const string SeekerOrOwnerOrAdmin = "SeekerOrOwnerOrAdmin";
}
