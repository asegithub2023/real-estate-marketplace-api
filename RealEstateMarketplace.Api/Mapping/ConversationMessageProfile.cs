using AutoMapper;
using RealEstateMarketplace.Application.Conversations.Commands;
using RealEstateMarketplace.Application.Messages.Commands;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Domain.Entities;

namespace RealEstateMarketplace.Api.Mapping;

public sealed class ConversationMessageProfile : Profile
{
    public ConversationMessageProfile()
    {
        CreateMap<CreateConversationDto, CreateConversationCommand>();
        CreateMap<Conversation, ConversationDto>();
        CreateMap<UpdateConversationDto, UpdateConversationCommand>();

        CreateMap<CreateMessageDto, CreateMessageCommand>();
        CreateMap<Message, MessageDto>();
        CreateMap<UpdateMessageDto, UpdateMessageCommand>();
    }
}
