namespace RealEstateMarketplace.Application.DTOs;

public class FavoriteDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}

public class CreateFavoriteDto
{
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}
