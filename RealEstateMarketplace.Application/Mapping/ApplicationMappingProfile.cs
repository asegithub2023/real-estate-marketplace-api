using AutoMapper;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Application.Mapping;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<User, AuthResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        CreateMap<Property, PropertyDto>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner != null ? src.Owner.FullName : string.Empty))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
            .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.PropertyFeatures));

        CreateMap<CreatePropertyDto, Property>();
        CreateMap<UpdatePropertyDto, Property>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
            .ForMember(dest => dest.Owner, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.Favorites, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyFeatures, opt => opt.Ignore())
            .ForMember(dest => dest.Reviews, opt => opt.Ignore())
            .ForMember(dest => dest.Reports, opt => opt.Ignore())
            .ForMember(dest => dest.Conversations, opt => opt.Ignore());

        CreateMap<PropertyImage, PropertyImageDto>();
        CreateMap<CreatePropertyImageDto, PropertyImage>();
        CreateMap<UpdatePropertyImageDto, PropertyImage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyId, opt => opt.Ignore())
            .ForMember(dest => dest.Property, opt => opt.Ignore());

        CreateMap<PropertyFeature, PropertyFeatureDto>();
        CreateMap<CreatePropertyFeatureDto, PropertyFeature>();
        CreateMap<UpdatePropertyFeatureDto, PropertyFeature>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Properties, opt => opt.Ignore());

        CreateMap<Favorite, FavoriteDto>();
        CreateMap<CreateFavoriteDto, Favorite>();

        CreateMap<Review, ReviewDto>();
        CreateMap<CreateReviewDto, Review>();
        CreateMap<UpdateReviewDto, Review>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Property, opt => opt.Ignore());

        CreateMap<Notification, NotificationDto>();
        CreateMap<CreateNotificationDto, Notification>();
        CreateMap<UpdateNotificationDto, Notification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<Report, ReportDto>();
        CreateMap<CreateReportDto, Report>();
        CreateMap<UpdateReportDto, Report>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Property, opt => opt.Ignore());

        CreateMap<Conversation, ConversationDto>();
        CreateMap<CreateConversationDto, Conversation>();
        CreateMap<UpdateConversationDto, Conversation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Property, opt => opt.Ignore())
            .ForMember(dest => dest.Buyer, opt => opt.Ignore())
            .ForMember(dest => dest.Owner, opt => opt.Ignore())
            .ForMember(dest => dest.Messages, opt => opt.Ignore());

        CreateMap<Message, MessageDto>();
        CreateMap<CreateMessageDto, Message>();
        CreateMap<UpdateMessageDto, Message>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ConversationId, opt => opt.Ignore())
            .ForMember(dest => dest.SenderId, opt => opt.Ignore())
            .ForMember(dest => dest.Conversation, opt => opt.Ignore())
            .ForMember(dest => dest.Sender, opt => opt.Ignore());
    }
}
