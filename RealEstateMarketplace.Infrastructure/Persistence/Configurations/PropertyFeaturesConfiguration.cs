using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Infrastructure.Persistence.Configurations;

public class PropertyFeatureConfiguration : IEntityTypeConfiguration<PropertyFeature>
{
    public void Configure(EntityTypeBuilder<PropertyFeature> builder)
    {
        builder.HasKey(pf => pf.Id);

        builder.Property(pf => pf.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pf => pf.Icon)
            .HasMaxLength(100);

        builder.HasIndex(pf => pf.Name)
            .IsUnique();

        builder.HasMany(pf => pf.Properties)
            .WithMany(p => p.PropertyFeatures);
    }
}
