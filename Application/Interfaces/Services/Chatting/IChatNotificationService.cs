// Application/Interfaces/Services/Chatting/IChatNotificationService.cs
using System;
using System.Threading.Tasks;
using Domain.Entities.Chatting;

namespace Application.Interfaces.Services.Chatting
{
    public interface IChatNotificationService
    {
        Task NotifyMessageSent(Guid conversationId, Message message);
        Task NotifyUserJoinedConversation(Guid conversationId, string userId, string userName);
        Task NotifyUserLeftConversation(Guid conversationId, string userId, string userName);
    }
}
