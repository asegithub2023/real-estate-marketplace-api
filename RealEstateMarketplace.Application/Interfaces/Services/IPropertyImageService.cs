using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Interfaces.Services;

public interface IPropertyImageService
{
    Task<IReadOnlyList<PropertyImageDto>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
    Task<PropertyImageDto> CreateAsync(CreatePropertyImageDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
