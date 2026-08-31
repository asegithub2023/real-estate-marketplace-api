using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Interfaces.Services;

public interface IReportService
{
    Task<ReportDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReportDto>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
    Task<ReportDto> CreateAsync(CreateReportDto request, int userId, CancellationToken cancellationToken = default);
    Task<ReportDto?> UpdateAsync(int id, UpdateReportDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
