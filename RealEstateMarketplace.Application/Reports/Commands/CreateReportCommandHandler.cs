using MediatR;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Mapping;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Reports.Commands;

public sealed class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, ReportDto>
{
    private readonly IReportRepository _reportRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPropertyRepository _propertyRepository;

    public CreateReportCommandHandler(
        IReportRepository reportRepository,
        IUserRepository userRepository,
        IPropertyRepository propertyRepository)
    {
        _reportRepository = reportRepository;
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<ReportDto> Handle(CreateReportCommand request, CancellationToken cancellationToken)
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

        var created = await _reportRepository.GetByIdWithDetailsAsync(report.Id, cancellationToken);
        return created!.ToDto();
    }
}