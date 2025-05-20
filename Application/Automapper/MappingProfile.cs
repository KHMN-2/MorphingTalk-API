using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chatting;
using Application.Interfaces.Services.Authentication;
using AutoMapper;
using Domain.Entities.Chatting;
using Domain.Entities.Users;
using MorphingTalk_API.DTOs.Chatting;

namespace Application.Automapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile(){
            //CreateMap<Conversation, ChatDto>()
            //    .ForMember(dest => dest.UserIds, opt => opt.MapFrom(src => src.ChatUsers.Select(cu => cu.UserId)));

            
            CreateMap<TextMessage, MessageDto>();
            CreateMap<User, UserDto>();
            // If you need to map DTOs back to domain models
            //CreateMap<ChatDto, Conversation>()
            //    .ForMember(dest => dest.ChatUsers, opt => opt.Ignore()); // Handle this separately if needed

            CreateMap<MessageDto, TextMessage>();
        }
    }
}
