namespace RealEstateMarketplace.Application.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}

public class CreateReviewDto
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}

public class UpdateReviewDto
{
    public int? Rating { get; set; }
    public string? Comment { get; set; }
}
