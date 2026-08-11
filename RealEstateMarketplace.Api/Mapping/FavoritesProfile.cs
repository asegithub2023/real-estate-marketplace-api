using AutoMapper;
using RealEstateMarketplace.Application.Favorites.Commands;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Api.Mapping;

public sealed class FavoritesProfile : Profile
{
    public FavoritesProfile()
    {
        CreateMap<CreateFavoriteDto, AddFavoriteCommand>();
        CreateMap<Favorite, FavoriteDto>();
    }
}
