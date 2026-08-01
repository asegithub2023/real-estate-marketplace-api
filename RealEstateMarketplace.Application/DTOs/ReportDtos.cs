namespace RealEstateMarketplace.Application.DTOs;

public class ReportDto
{
    public int Id { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}

public class CreateReportDto
{
    public required string Reason { get; set; }
    public int UserId { get; set; }
    public int PropertyId { get; set; }
}

public class UpdateReportDto
{
    public string? Reason { get; set; }
}
