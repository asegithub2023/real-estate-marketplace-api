using AutoMapper;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Infrastructure.Services;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public PropertyService(IPropertyRepository propertyRepository, IUserRepository userRepository, IMapper mapper)
    {
        _propertyRepository = propertyRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PropertyDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(id, cancellationToken);
        return property is null ? null : _mapper.Map<PropertyDto>(property);
    }

    public async Task<IReadOnlyList<PropertyDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var properties = await _propertyRepository.GetAllAsync(cancellationToken);
        return properties.Select(property => _mapper.Map<PropertyDto>(property)).ToList();
    }

    public async Task<IReadOnlyList<PropertyDto>> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default)
    {
        var properties = await _propertyRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
        return properties.Select(property => _mapper.Map<PropertyDto>(property)).ToList();
    }

    public async Task<PropertyDto> CreateAsync(CreatePropertyDto request, CancellationToken cancellationToken = default)
    {
        var owner = await _userRepository.GetByIdAsync(request.OwnerId, cancellationToken);
        if (owner is null)
        {
            throw new InvalidOperationException("Owner was not found.");
        }

        var property = new Property
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            City = request.City,
            Address = request.Address,
            Country = request.Country,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            Rooms = request.Rooms,
            Area = request.Area,
            Status = request.Status,
            OwnerId = request.OwnerId
        };

        await _propertyRepository.AddAsync(property, cancellationToken);
        return _mapper.Map<PropertyDto>(property);
    }

    public async Task<PropertyDto?> UpdateAsync(int id, UpdatePropertyDto request, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(id, cancellationToken);
        if (property is null)
        {
            return null;
        }

        if (request.Title is not null)
        {
            property.Title = request.Title;
        }

        if (request.Description is not null)
        {
            property.Description = request.Description;
        }

        if (request.Price is not null)
        {
            property.Price = request.Price.Value;
        }

        if (request.City is not null)
        {
            property.City = request.City;
        }

        if (request.Address is not null)
        {
            property.Address = request.Address;
        }

        if (request.Country is not null)
        {
            property.Country = request.Country;
        }

        if (request.Bedrooms is not null)
        {
            property.Bedrooms = request.Bedrooms.Value;
        }

        if (request.Bathrooms is not null)
        {
            property.Bathrooms = request.Bathrooms.Value;
        }

        if (request.Rooms is not null)
        {
            property.Rooms = request.Rooms.Value;
        }

        if (request.Area is not null)
        {
            property.Area = request.Area.Value;
        }

        if (request.Status is not null)
        {
            property.Status = request.Status.Value;
        }

        await _propertyRepository.UpdateAsync(property, cancellationToken);
        return _mapper.Map<PropertyDto>(property);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetByIdAsync(id, cancellationToken);
        if (property is null)
        {
            return false;
        }

        await _propertyRepository.DeleteAsync(property, cancellationToken);
        return true;
    }
}
