using AutoMapper;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Infrastructure.Services;

public class PropertyFeatureService : IPropertyFeatureService
{
    private readonly IPropertyFeatureRepository _propertyFeatureRepository;
    private readonly IMapper _mapper;

    public PropertyFeatureService(IPropertyFeatureRepository propertyFeatureRepository, IMapper mapper)
    {
        _propertyFeatureRepository = propertyFeatureRepository;
        _mapper = mapper;
    }

    public async Task<PropertyFeatureDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var feature = await _propertyFeatureRepository.GetByIdAsync(id, cancellationToken);
        return feature is null ? null : _mapper.Map<PropertyFeatureDto>(feature);
    }

    public async Task<IReadOnlyList<PropertyFeatureDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var features = await _propertyFeatureRepository.GetAllAsync(cancellationToken);
        return features.Select(feature => _mapper.Map<PropertyFeatureDto>(feature)).ToList();
    }

    public async Task<PropertyFeatureDto> CreateAsync(CreatePropertyFeatureDto request, CancellationToken cancellationToken = default)
    {
        var existingFeature = await _propertyFeatureRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingFeature is not null)
        {
            throw new InvalidOperationException("A feature with this name already exists.");
        }

        var feature = new PropertyFeature
        {
            Name = request.Name,
            Icon = request.Icon
        };

        await _propertyFeatureRepository.AddAsync(feature, cancellationToken);
        return _mapper.Map<PropertyFeatureDto>(feature);
    }

    public async Task<PropertyFeatureDto?> UpdateAsync(int id, UpdatePropertyFeatureDto request, CancellationToken cancellationToken = default)
    {
        var feature = await _propertyFeatureRepository.GetByIdAsync(id, cancellationToken);
        if (feature is null)
        {
            return null;
        }

        if (request.Name is not null)
        {
            feature.Name = request.Name;
        }

        if (request.Icon is not null)
        {
            feature.Icon = request.Icon;
        }

        await _propertyFeatureRepository.UpdateAsync(feature, cancellationToken);
        return _mapper.Map<PropertyFeatureDto>(feature);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var feature = await _propertyFeatureRepository.GetByIdAsync(id, cancellationToken);
        if (feature is null)
        {
            return false;
        }

        await _propertyFeatureRepository.DeleteAsync(id, cancellationToken);
        return true;
    }
}
