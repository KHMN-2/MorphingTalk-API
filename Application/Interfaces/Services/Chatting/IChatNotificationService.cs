// Application/Interfaces/Services/Chatting/IChatNotificationService.cs
using System;
using System.Threading.Tasks;
using Domain.Entities.Chatting;

namespace Application.Interfaces.Services.Chatting
{
    public interface IChatNotificationService
    {
        Task NotifyMessageSent(Guid conversationId, Message message);
        Task NotifyUserJoinedConversation(Guid conversationId, string userId, string displayName);
        Task NotifyUserLeftConversation(Guid conversationId, string userId, string displayName);
        Task NotifyMessageTranslated(Guid conversationId, Guid translatedMessageId, string senderId, string targetLanguage);
        Task NotifyVoiceTrainingCompleted(string userId, bool success, string modelId, string? errorMessage = null);
        Task NotifyMessageDeleted(Guid conversationId, Guid messageId, string deletedByUserId);
    }
}
