using AutoMapper;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IMapper _mapper;

    public ReportService(IReportRepository reportRepository, IUserRepository userRepository, IPropertyRepository propertyRepository, IMapper mapper)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
        _mapper = mapper;
    }

    public async Task<ReportDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var report = await _reportRepository.GetByIdAsync(id, cancellationToken);
        return report is null ? null : _mapper.Map<ReportDto>(report);
    }

    public async Task<IReadOnlyList<ReportDto>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        var reports = await _reportRepository.GetByPropertyIdAsync(propertyId, cancellationToken);
        return reports.Select(report => _mapper.Map<ReportDto>(report)).ToList();
    }

    public async Task<ReportDto> CreateAsync(CreateReportDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (user is null || property is null)
        {
            throw new InvalidOperationException("User or property was not found.");
        }

        var report = new Report
        {
            Reason = request.Reason,
            UserId = request.UserId,
            PropertyId = request.PropertyId
        };

        await _reportRepository.AddAsync(report, cancellationToken);
        return _mapper.Map<ReportDto>(report);
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
        return _mapper.Map<ReportDto>(report);
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
