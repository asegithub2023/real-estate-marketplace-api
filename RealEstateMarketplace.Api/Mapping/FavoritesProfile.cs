using AutoMapper;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Api.Mapping;

public sealed class FavoritesProfile : Profile
{
    public FavoritesProfile()
    {
        CreateMap<Favorite, FavoriteDto>()
            .ForMember(dest => dest.PropertyTitle, opt => opt.MapFrom(src => src.Property.Title))
            .ForMember(dest => dest.PropertyPrice, opt => opt.MapFrom(src => src.Property.Price))
            .ForMember(dest => dest.PropertyCity, opt => opt.MapFrom(src => src.Property.City))
            .ForMember(dest => dest.PropertyCountry, opt => opt.MapFrom(src => src.Property.Country))
            .ForMember(dest => dest.PropertyBedrooms, opt => opt.MapFrom(src => src.Property.Bedrooms))
            .ForMember(dest => dest.PropertyBathrooms, opt => opt.MapFrom(src => src.Property.Bathrooms))
            .ForMember(dest => dest.PropertyArea, opt => opt.MapFrom(src => src.Property.Area))
            .ForMember(dest => dest.PropertyImageUrl, opt => opt.MapFrom(src =>
                src.Property.Images.Select(i => i.ImageUrl).FirstOrDefault()));
    }
}