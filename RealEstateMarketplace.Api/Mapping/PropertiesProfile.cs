using AutoMapper;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Properties.Commands;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Api.Mapping;

public sealed class PropertiesProfile : Profile
{
    public PropertiesProfile()
    {
        CreateMap<PropertyImage, PropertyImageDto>();
        CreateMap<PropertyFeature, PropertyFeatureDto>();
        CreateMap<Property, PropertyDto>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.FullName));
        CreateMap<CreatePropertyDto, CreatePropertyCommand>();
    }
}
