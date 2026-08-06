using MediatR;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Repositories;
using RealEstateMarketplace.Application.Mapping;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Properties.Commands;

public sealed class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, Result<PropertyDto, PropertyError>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUserRepository _userRepository;

    public CreatePropertyCommandHandler(
        IPropertyRepository propertyRepository,
        IUserRepository userRepository)
    {
        _propertyRepository = propertyRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<PropertyDto, PropertyError>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
    {
        var owner = await _userRepository.GetByIdAsync(request.OwnerId, cancellationToken);
        if (owner is null)
        {
            return Result.Failure<PropertyDto, PropertyError>(PropertyError.OwnerNotFound(request.OwnerId));
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
        return Result.Success<PropertyDto, PropertyError>(property.ToDto());
    }
}
