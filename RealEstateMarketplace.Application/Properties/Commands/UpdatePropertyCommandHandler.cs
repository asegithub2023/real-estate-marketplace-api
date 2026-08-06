using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Mapping;

namespace RealEstateMarketplace.Application.Properties.Commands;

public sealed class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, Result<PropertyDto, PropertyError>>
{
    private readonly IPropertyRepository _propertyRepository;

    public UpdatePropertyCommandHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<Result<PropertyDto, PropertyError>> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (property is null)
        {
            return Result.Failure<PropertyDto, PropertyError>(PropertyError.NotFound(request.Id));
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
        return Result.Success<PropertyDto, PropertyError>(property.ToDto());
    }
}
