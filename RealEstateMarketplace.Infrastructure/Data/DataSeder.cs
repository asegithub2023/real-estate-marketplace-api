using Microsoft.EntityFrameworkCore;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Infrastructure.Persistence;

public static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PropertyFeature>().HasData(
            new PropertyFeature { Id = 1, Name = "Parking", Icon = "bi-car-front" },
            new PropertyFeature { Id = 2, Name = "Swimming Pool", Icon = "bi-water" },
            new PropertyFeature { Id = 3, Name = "Garden", Icon = "bi-tree" },
            new PropertyFeature { Id = 4, Name = "Balcony", Icon = "bi-building" },
            new PropertyFeature { Id = 5, Name = "Security", Icon = "bi-shield-check" },
            new PropertyFeature { Id = 6, Name = "Elevator", Icon = "bi-arrow-up-square" },
            new PropertyFeature { Id = 7, Name = "Internet", Icon = "bi-wifi" },
            new PropertyFeature { Id = 8, Name = "Air Conditioning", Icon = "bi-snow" },
            new PropertyFeature { Id = 9, Name = "Gym", Icon = "bi-heart-pulse" },
            new PropertyFeature { Id = 10, Name = "Furnished", Icon = "bi-house-check" }
        );
    }
}
