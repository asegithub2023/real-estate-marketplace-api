using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Application.Mapping;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPropertyRepository _propertyRepository;

    public ReportService(IReportRepository reportRepository, IUserRepository userRepository, IPropertyRepository propertyRepository)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<ReportDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return report is null ? null : report.ToDto();
    }

    public async Task<IReadOnlyList<ReportDto>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        var reports = await _reportRepository.GetByPropertyIdAsync(propertyId, cancellationToken);
        return reports.Select(report => report.ToDto()).ToList();
    }

    public async Task<ReportDto> CreateAsync(CreateReportDto request, int userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (user is null || property is null)
        {
            throw new InvalidOperationException("User or property was not found.");
        }

        var report = new Report
        {
            Reason = request.Reason,
            UserId = userId,
            PropertyId = request.PropertyId
        };

        await _reportRepository.AddAsync(report, cancellationToken);

        var created = await _reportRepository.GetByIdWithDetailsAsync(report.Id, cancellationToken);
        return created!.ToDto();
    }

    public async Task<ReportDto?> UpdateAsync(int id, UpdateReportDto request, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetByIdAsync(id, cancellationToken);
        if (report is null)
        {
            return null;
        }

        if (request.Reason is not null)
        {
            report.Reason = request.Reason;
        }

        await _reportRepository.UpdateAsync(report, cancellationToken);

        var updated = await _reportRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return updated?.ToDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetByIdAsync(id, cancellationToken);
        if (report is null)
        {
            return false;
        }

        await _reportRepository.DeleteAsync(report, cancellationToken);
        return true;
    }
}