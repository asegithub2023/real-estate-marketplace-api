using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Mapping;

public static class DtoMappingExtensions
{
    public static AuthResponseDto ToDto(this User source)
        => new()
        {
            UserId = source.Id,
            FullName = source.FullName,
            Email = source.Email,
            Role = source.Role.ToString(),
        };

    public static PropertyDto ToDto(this Property source)
        => new()
        {
            Id = source.Id,
            Title = source.Title,
            Description = source.Description,
            Price = source.Price,
            City = source.City,
            Address = source.Address,
            Country = source.Country,
            Bedrooms = source.Bedrooms,
            Bathrooms = source.Bathrooms,
            Rooms = source.Rooms,
            Area = source.Area,
            Status = source.Status,
            OwnerId = source.OwnerId,
            OwnerName = source.Owner?.FullName ?? string.Empty,
            Images = source.Images.Select(image => image.ToDto()).ToList(),
            Features = source.PropertyFeatures.Select(feature => feature.ToDto()).ToList(),
        };

    public static PropertyImageDto ToDto(this PropertyImage source)
        => new()
        {
            Id = source.Id,
            ImageUrl = source.ImageUrl,
            PropertyId = source.PropertyId,
        };

    public static PropertyFeatureDto ToDto(this PropertyFeature source)
        => new()
        {
            Id = source.Id,
            Name = source.Name,
            Icon = source.Icon,
        };

    public static FavoriteDto ToDto(this Favorite source)
        => new()
        {
            Id = source.Id,
            UserId = source.UserId,
            PropertyId = source.PropertyId,
        };

    public static ReviewDto ToDto(this Review source)
        => new()
        {
            Id = source.Id,
            Rating = source.Rating,
            Comment = source.Comment,
            UserId = source.UserId,
            PropertyId = source.PropertyId,
        };

    public static NotificationDto ToDto(this Notification source)
        => new()
        {
            Id = source.Id,
            Title = source.Title,
            Message = source.Message,
            IsRead = source.IsRead,
            UserId = source.UserId,
        };

    public static ReportDto ToDto(this Report source)
        => new()
        {
            Id = source.Id,
            Reason = source.Reason,
            UserId = source.UserId,
            PropertyId = source.PropertyId,
        };

    public static ConversationDto ToDto(this Conversation source)
        => new()
        {
            Id = source.Id,
            PropertyId = source.PropertyId,
            BuyerId = source.BuyerId,
            OwnerId = source.OwnerId,
            CreatedAt = source.CreatedAt,
        };

    public static MessageDto ToDto(this Message source)
        => new()
        {
            Id = source.Id,
            ConversationId = source.ConversationId,
            SenderId = source.SenderId,
            Content = source.Content,
            SentAt = source.SentAt,
        };
}
