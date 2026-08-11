using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Interfaces.Services;

public interface ICachedPropertyService
{
    Task<PropertyDto?> GetPropertyAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PropertyDto>> GetAllPropertiesAsync(CancellationToken cancellationToken = default);
    Task InvalidatePropertyCacheAsync(int? id, CancellationToken cancellationToken = default);
}
