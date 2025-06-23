// Application/Services/Chatting/SignalRChatNotificationService.cs
using Application.DTOs.Chatting;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Microsoft.AspNetCore.SignalR;
using MorphingTalk_API.DTOs.Chatting;
using MorphingTalk_API.Hubs;
using System;
using System.Threading.Tasks;


namespace Application.Services.Chatting
{
    public class SignalRChatNotificationService : IChatNotificationService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRChatNotificationService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyMessageSent(Guid conversationId, Message message)
        {
            var messageDto = new MessageSummaryDto
            {
                Id = message.Id,
                Type = message is TextMessage ? MessageType.Text.ToString() : message is VoiceMessage ? MessageType.Voice.ToString() : "unknown",
                SenderId = message.ConversationUser?.UserId,
                SenderDisplayName = message.ConversationUser?.User?.FullName,
                Text = message is TextMessage tm ? tm.Content : null,
                SentAt = message.SentAt,
                ConversationId = message.ConversationId.ToString(),
                MessageStatus = message.Status.ToString(),
                VoiceFileUrl = message is VoiceMessage vm ? (vm.IsTranslated ? vm.TranslatedVoiceUrl : vm.VoiceUrl) : null,
                Duration = message is VoiceMessage v ? v.DurationSeconds : null,
            };

            await _hubContext.Clients.Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", messageDto);
        }

        public async Task NotifyUserJoinedConversation(Guid conversationId, string userId, string userName)
        {
            await _hubContext.Clients.Group(conversationId.ToString())
                .SendAsync("UserJoined", conversationId, userId, userName);
        }

        public async Task NotifyUserLeftConversation(Guid conversationId, string userId, string userName)
        {
            await _hubContext.Clients.Group(conversationId.ToString())
                .SendAsync("UserLeft", conversationId, userId, userName);
        }        Task IChatNotificationService.NotifyMessageTranslated(Guid conversationId, Guid translatedMessageId, string senderId, string targetLanguage)
        {
            throw new NotImplementedException();
        }

        public async Task NotifyVoiceTrainingCompleted(string userId, bool success, string modelId, string? errorMessage = null)
        {
            var notification = new
            {
                success = success,
                modelId = modelId,
                timestamp = DateTime.UtcNow,
                message = success ? "Voice training completed successfully! Your voice model is now ready for use." 
                                 : $"Voice training failed: {errorMessage ?? "Unknown error"}"
            };

            await _hubContext.Clients.User(userId)
                .SendAsync("VoiceTrainingCompleted", notification);
        }
    }
}
