using AutoMapper;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Infrastructure.Services;

public class PropertyImageService : IPropertyImageService
{
    private readonly IPropertyImageRepository _propertyImageRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IMapper _mapper;

    public PropertyImageService(IPropertyImageRepository propertyImageRepository, IPropertyRepository propertyRepository, IMapper mapper)
    {
        _propertyImageRepository = propertyImageRepository;
        _propertyRepository = propertyRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PropertyImageDto>> GetByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        var images = await _propertyImageRepository.GetByPropertyIdAsync(propertyId, cancellationToken);
        return images.Select(image => _mapper.Map<PropertyImageDto>(image)).ToList();
    }

    public async Task<PropertyImageDto> CreateAsync(CreatePropertyImageDto request, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (property is null)
        {
            throw new InvalidOperationException("Property was not found.");
        }

        var image = new PropertyImage
        {
            ImageUrl = request.ImageUrl,
            PropertyId = request.PropertyId
        };

        await _propertyImageRepository.AddAsync(image, cancellationToken);
        return _mapper.Map<PropertyImageDto>(image);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var image = await _propertyImageRepository.GetByIdAsync(id, cancellationToken);
        if (image is null)
        {
            return false;
        }

        await _propertyImageRepository.DeleteAsync(id, cancellationToken);
        return true;
    }
}
