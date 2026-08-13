using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Interfaces.Services;

public interface IPropertyService
{
    Task<PropertyDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PropertyDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PropertyDto>> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default);
    Task<PagedResponse<PropertyDto>> GetPropertiesAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<PropertyDto> CreateAsync(CreatePropertyDto request, CancellationToken cancellationToken = default);
    Task<PropertyDto?> UpdateAsync(int id, UpdatePropertyDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
