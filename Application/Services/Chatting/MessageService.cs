using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chatting;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MorphingTalk_API.DTOs.Chatting;

namespace Application.Services.Chatting
{
    public class MessageService : IMessageService
    {
        private readonly IEnumerable<IMessageHandler> _handlers;
        private readonly ILogger<MessageService> _logger;
        private readonly IConversationUserRepository _conversationUserRepo;
        private readonly IMessageRepository _messageRepository;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly IMemoryCache _memoryCache;       
        public MessageService(IEnumerable<IMessageHandler> handlers, IConversationUserRepository conversationUserRepository, IMessageRepository messageRepository, IChatNotificationService chatNotificationService, IMemoryCache memoryCache, ILogger<MessageService> logger)
        {
            _handlers = handlers;
            _conversationUserRepo = conversationUserRepository;
            _messageRepository = messageRepository;
            _chatNotificationService = chatNotificationService;
            _memoryCache = memoryCache;
            _logger = logger;
        }        
        public async Task<string> ProcessMessageAsync(SendMessageDto message, Guid conversationId, string userId)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID is required", nameof(userId));

            var handler = _handlers.FirstOrDefault(h => h.CanHandle(message.Type));
            var cu = await _conversationUserRepo.GetByIdsAsync(conversationId, userId);
            if (cu == null)
                throw new KeyNotFoundException("ConversationUser not found");
            if (handler == null)
            {
                _logger.LogError($"No handler found for message type: {message.Type}");
                throw new NotSupportedException($"Message type {message.Type} is not supported");
            }
            
            // Set the correct ConversationUser ID
            message.SenderConversationUserId = cu.Id;
            
            if (message.NeedTranslation)
            {
                if(message.Type == MessageType.Text)
                {
                    // For text messages, we handle translation directly
                    await handler.HandleTranslationAsync(message, conversationId, userId);
                }
                else
                {
                    var taskId = await handler.HandleTranslationAsync(message, conversationId, userId);
                    VoiceMessage voiceMessage = new VoiceMessage
                    {
                        Id = Guid.NewGuid(),
                        ConversationId = conversationId,
                        ConversationUserId = message.SenderConversationUserId,
                        VoiceUrl = message.VoiceFileUrl ?? throw new ArgumentException("Voice file URL is required"),
                        DurationSeconds = message.DurationSeconds ?? 0,
                        SentAt = DateTime.UtcNow,
                        IsTranslated = false,
                        Status = MessageStatus.Sent
                    };
                    
                    // Save the message to repository first
                    await _messageRepository.AddAsync(voiceMessage);
                    
                    // Cache the message for webhook processing
                    _memoryCache.Set(taskId, voiceMessage, TimeSpan.FromMinutes(30));
                    
                    // **IMPORTANT: Send initial notification that voice message was sent and is being translated**
                    await _chatNotificationService.NotifyMessageSent(conversationId, voiceMessage);
                    
                    _logger.LogInformation($"Voice message sent and translation started. TaskId: {taskId}, ConversationId: {conversationId}");
                    
                    return taskId; // Return the task ID for the translation process
                }
            }

            await handler.HandleMessageAsync(message, conversationId, userId);
            
            // Get the newly created message from repository to send notification
            var messages = await _messageRepository.GetMessagesForConversationAsync(conversationId, 1, 0);
            var latestMessage = messages.FirstOrDefault();

            if (latestMessage != null)
            {
                // Notify connected clients about the new message
                await _chatNotificationService.NotifyMessageSent(conversationId, latestMessage);
            }

            return "";
        }
    }
}
