using RealEstateMarketplace.Domain.Enums;

public class ReportDto
{
    public int Id { get; set; }
    public string Reason { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserId { get; set; }
    public string ReporterName { get; set; } = string.Empty;
    public int PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
}

public class CreateReportDto
{
    public required string Reason { get; set; }
    public int PropertyId { get; set; }
}

public class UpdateReportDto
{
    public string? Reason { get; set; }
}

public class UpdateReportStatusDto
{
    public ReportStatus Status { get; set; }
}
