namespace RealEstateMarketplace.Domain.Entities;

using RealEstateMarketplace.Domain.Enums;

public class Property
{
    public int Id { get; set; }

    public required string Title { get; set; }
    public required string Description { get; set; }

    public decimal Price { get; set; }

    public string City { get; set; } = "";
    public string Address { get; set; } = "";
    public string Country { get; set; } = "";

    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Rooms { get; set; }
    public double Area { get; set; }

    public PropertyStatus Status { get; set; }

    public PropertyType PropertyType { get; set; }
    public ListingType ListingType { get; set; }

    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public ICollection<PropertyImage> Images { get; set; } = [];
    public ICollection<Favorite> Favorites { get; set; } = [];
    public ICollection<PropertyFeature> PropertyFeatures { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Report> Reports { get; set; } = [];
    public ICollection<Conversation> Conversations { get; set; } = [];
}