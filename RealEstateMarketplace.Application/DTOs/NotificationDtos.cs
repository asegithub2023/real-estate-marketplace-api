namespace RealEstateMarketplace.Application.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public int UserId { get; set; }
}

public class CreateNotificationDto
{
    public required string Title { get; set; }
    public required string Message { get; set; }
    public bool IsRead { get; set; }
    public int UserId { get; set; }
}

public class UpdateNotificationDto
{
    public string? Title { get; set; }
    public string? Message { get; set; }
    public bool? IsRead { get; set; }
}
