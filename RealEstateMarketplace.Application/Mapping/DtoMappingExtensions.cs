using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Mapping;

public static class DtoMappingExtensions
{
    public static FavoriteDto ToDto(this Favorite favorite) => new()
    {
        Id = favorite.Id,
        UserId = favorite.UserId,
        PropertyId = favorite.PropertyId
    };

    public static AuthResponseDto ToDto(this User user) => new()
    {
        UserId = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        Role = user.Role.ToString()
    };

    public static PropertyDto ToDto(this Property property) => new()
    {
        Id = property.Id,
        Title = property.Title,
        Description = property.Description,
        Price = property.Price,
        City = property.City,
        Address = property.Address,
        Country = property.Country,
        Bedrooms = property.Bedrooms,
        Bathrooms = property.Bathrooms,
        Rooms = property.Rooms,
        Area = property.Area,
        Status = property.Status,
        OwnerId = property.OwnerId,
        OwnerName = property.Owner?.FullName ?? string.Empty,
        Images = property.Images?.Select(image => image.ToDto()).ToList() ?? new List<PropertyImageDto>(),
        Features = property.PropertyFeatures?.Select(feature => feature.ToDto()).ToList() ?? new List<PropertyFeatureDto>()
    };

    public static PropertyImageDto ToDto(this PropertyImage image) => new()
    {
        Id = image.Id,
        ImageUrl = image.ImageUrl,
        PropertyId = image.PropertyId
    };

    public static PropertyFeatureDto ToDto(this PropertyFeature feature) => new()
    {
        Id = feature.Id,
        Name = feature.Name,
        Icon = feature.Icon
    };

    public static ConversationDto ToDto(this Conversation conversation) => new()
    {
        Id = conversation.Id,
        PropertyId = conversation.PropertyId,
        BuyerId = conversation.BuyerId,
        OwnerId = conversation.OwnerId,
        CreatedAt = conversation.CreatedAt
    };

    public static MessageDto ToDto(this Message message) => new()
    {
        Id = message.Id,
        ConversationId = message.ConversationId,
        SenderId = message.SenderId,
        Content = message.Content,
        SentAt = message.SentAt
    };

    public static NotificationDto ToDto(this Notification notification) => new()
    {
        Id = notification.Id,
        Title = notification.Title,
        Message = notification.Message,
        IsRead = notification.IsRead,
        UserId = notification.UserId
    };

    public static ReportDto ToDto(this Report report) => new()
    {
        Id = report.Id,
        Reason = report.Reason,
        UserId = report.UserId,
        PropertyId = report.PropertyId
    };

    public static ReviewDto ToDto(this Review review) => new()
    {
        Id = review.Id,
        Rating = review.Rating,
        Comment = review.Comment,
        UserId = review.UserId,
        PropertyId = review.PropertyId
    };
}
