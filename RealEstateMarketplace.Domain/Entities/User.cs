using Microsoft.AspNetCore.Identity;
using RealEstateMarketplace.Domain.Enums;

namespace RealEstateMarketplace.Domain.Entities;

public class User : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;

    public string? ProfileImageUrl { get; set; }

    public Role Role { get; set; }

    public ICollection<Property> Properties { get; set; } = [];
    public ICollection<Favorite> Favorites { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<Report> Reports { get; set; } = [];
    public ICollection<Message> SentMessages { get; set; } = [];
    public ICollection<Conversation> Conversations { get; set; } = [];
}


