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
            var messageDto = MessageSummaryDto.FromMessage(message);

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
        }        
        public async Task NotifyMessageTranslated(Guid conversationId, Guid translatedMessageId, string senderId, string targetLanguage)
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

        public async Task NotifyMessageDeleted(Guid conversationId, Guid messageId, string deletedByUserId)
        {
            var deleteNotification = new
            {
                messageId = messageId,
                deletedByUserId = deletedByUserId,
                timestamp = DateTime.UtcNow,
                type = "message_deleted"
            };

            _logger.LogInformation("Message deleted - ConversationId: {ConversationId}, MessageId: {MessageId}, DeletedByUserId: {DeletedByUserId}", conversationId, messageId, deletedByUserId);

            // Notify all clients in the conversation that a message has been deleted
            await _hubContext.Clients.Group(conversationId.ToString())
                .SendAsync("MessageDeleted", deleteNotification);
        }
    }
}


