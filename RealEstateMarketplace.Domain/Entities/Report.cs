using RealEstateMarketplace.Domain.Enums;

namespace RealEstateMarketplace.Domain.Entities;
public class Report
{
    public int Id { get; set; }

    public required string Reason { get; set; }

    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
}