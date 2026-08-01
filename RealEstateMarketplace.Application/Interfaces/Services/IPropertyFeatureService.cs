using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Interfaces.Services;

public interface IPropertyFeatureService
{
    Task<PropertyFeatureDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PropertyFeatureDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PropertyFeatureDto> CreateAsync(CreatePropertyFeatureDto request, CancellationToken cancellationToken = default);
    Task<PropertyFeatureDto?> UpdateAsync(int id, UpdatePropertyFeatureDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
