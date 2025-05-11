using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chatting;
using Domain.Entities.Chatting;
using MorphingTalk_API.DTOs.Chatting;

namespace Application.Interfaces.Services.Chatting
{
    public interface IMessageHandler
    {
        Task HandleMessageAsync(SendMessageDto message, Guid conversationId, string userId);
        bool CanHandle(MessageType messageType);
    }
}
