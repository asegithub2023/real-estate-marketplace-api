namespace RealEstateMarketplace.Application.Reports;

public sealed record ReportError(string Code, string Message)
{
    public static ReportError NotFound(Guid id) =>
        new("report_not_found", $"Report '{id}' was not found.");

    public static ReportError AlreadyReported(Guid propertyId) =>
        new("report_already_exists", $"Property '{propertyId}' has already been reported.");
}