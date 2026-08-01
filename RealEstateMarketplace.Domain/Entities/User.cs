using RealEstateMarketplace.Domain.Enums;

public class User
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string PhoneNumber { get; set; }

    public Role Role { get; set; }

    public ICollection<Property> Properties { get; set; } = [];
    public ICollection<Favorite> Favorites { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<Report> Reports { get; set; } = [];
    public ICollection<Message> SentMessages { get; set; } = [];
    public ICollection<Conversation> Conversations { get; set; } = [];
  
}



