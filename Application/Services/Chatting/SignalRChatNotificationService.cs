// Application/Services/Chatting/SignalRChatNotificationService.cs
using Application.DTOs.Chatting;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MorphingTalk_API.DTOs.Chatting;
using MorphingTalk_API.Hubs;
using System;
using System.Threading.Tasks;


namespace Application.Services.Chatting
{
    public class SignalRChatNotificationService : IChatNotificationService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<SignalRChatNotificationService> _logger;

        public SignalRChatNotificationService(IHubContext<ChatHub> hubContext, ILogger<SignalRChatNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
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

            _logger.LogInformation("Sending message notification for MessageId: {MessageId}, ConversationId: {ConversationId}", message.Id, conversationId);

            await _hubContext.Clients.Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", messageDto);
        }

        public async Task NotifyUserJoinedConversation(Guid conversationId, string userId, string userName)
        {
            _logger.LogInformation("User joined - ConversationId: {ConversationId}, UserId: {UserId}, UserName: {UserName}", conversationId, userId, userName);

            await _hubContext.Clients.Group(conversationId.ToString())
                .SendAsync("UserJoined", conversationId, userId, userName);
        }

        public async Task NotifyUserLeftConversation(Guid conversationId, string userId, string userName)
        {
            _logger.LogInformation("User left - ConversationId: {ConversationId}, UserId: {UserId}, UserName: {UserName}", conversationId, userId, userName);

            await _hubContext.Clients.Group(conversationId.ToString())
                .SendAsync("UserLeft", conversationId, userId, userName);
        }        public async Task NotifyMessageTranslated(Guid conversationId, Guid translatedMessageId, string senderId, string targetLanguage)
        {
            var translationNotification = new
            {
                messageId = translatedMessageId,
                senderId = senderId,
                targetLanguage = targetLanguage,
                timestamp = DateTime.UtcNow,
                type = "translation_completed"
            };

            _logger.LogInformation("Message translated - ConversationId: {ConversationId}, TranslatedMessageId: {TranslatedMessageId}, SenderId: {SenderId}, TargetLanguage: {TargetLanguage}", conversationId, translatedMessageId, senderId, targetLanguage);

            // Notify all clients in the conversation that a message translation is complete
            await _hubContext.Clients.Group(conversationId.ToString())
                .SendAsync("MessageTranslated", translationNotification);
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

            _logger.LogInformation("Voice training completed - UserId: {UserId}, Success: {Success}, ModelId: {ModelId}", userId, success, modelId);

            await _hubContext.Clients.User(userId)
                .SendAsync("VoiceTrainingCompleted", notification);
        }
    }
}
