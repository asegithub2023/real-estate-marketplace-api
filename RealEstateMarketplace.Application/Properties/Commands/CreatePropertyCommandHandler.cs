using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Properties.Commands;

public sealed class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, Result<Property, PropertyError>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPropertyImageRepository _propertyImageRepository;
    
    public CreatePropertyCommandHandler(
        IPropertyRepository propertyRepository,
        IUserRepository userRepository,
        IPropertyImageRepository propertyImageRepository)
    {
        _propertyRepository = propertyRepository;
        _userRepository = userRepository;
        _propertyImageRepository = propertyImageRepository;
    }

    public async Task<Result<Property, PropertyError>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var owner = await _userRepository.GetByIdAsync(request.OwnerId, cancellationToken);
        if (owner is null)
        {
            return Result.Failure<Property, PropertyError>(PropertyError.OwnerNotFound(request.OwnerId));
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
            // New listings always start out Available. The owner changes this
            // later (e.g. to Sold/Rented) from their My Properties page.
            Status = RealEstateMarketplace.Domain.Enums.PropertyStatus.Available,
            PropertyType = request.PropertyType,
            ListingType = request.ListingType,
            OwnerId = request.OwnerId
        };

        await _propertyRepository.AddAsync(property, cancellationToken);
        foreach (var imageUrl in request.ImageUrls)
        {
            var propertyImage = new PropertyImage
            {
                PropertyId = property.Id,
                ImageUrl = imageUrl
            };
            await _propertyImageRepository.AddAsync(propertyImage, cancellationToken);
        }
        return Result.Success<Property, PropertyError>(property);
    }
}