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
        private readonly IConversationRepository _conversationRepository;
        public MessageService(IEnumerable<IMessageHandler> handlers, IConversationUserRepository conversationUserRepository, IMessageRepository messageRepository, IChatNotificationService chatNotificationService, IMemoryCache memoryCache, ILogger<MessageService> logger, IConversationRepository conversationRepository)
        {
            _handlers = handlers;
            _conversationUserRepo = conversationUserRepository;
            _messageRepository = messageRepository;
            _chatNotificationService = chatNotificationService;
            _memoryCache = memoryCache;
            _logger = logger;
            _conversationRepository = conversationRepository;
        }        
        public async Task<string> ProcessMessageAsync(SendMessageDto message, Guid conversationId, string userId)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID is required", nameof(userId));

            var handler = _handlers.FirstOrDefault(h => h.CanHandle(message.Type));
            var cu = await _conversationUserRepo.GetByIdsAsync(conversationId, userId);
            var con = await _conversationRepository.GetByIdAsync(conversationId);
     
            if (cu == null)
                throw new KeyNotFoundException("ConversationUser not found");
            if (handler == null)
            {
                _logger.LogError($"No handler found for message type: {message.Type}");
                throw new NotSupportedException($"Message type {message.Type} is not supported");
            }
            
            // Set the correct ConversationUser ID
            message.SenderConversationUserId = cu.Id;
            List<string> targetLanguages = new List<string>();
            foreach(var cuu in con.ConversationUsers)
            {

                if(!targetLanguages.Contains(cuu.User.NativeLanguage ?? "en"))
                    targetLanguages.Add(cuu.User.NativeLanguage ?? "en");
                
            }
        
            if (message.NeedTranslation)
            {
                if(message.Type == MessageType.Text)
                {
                    // For text messages, we handle translation directly
                    await handler.HandleTranslationAsync(message, conversationId, userId, targetLanguages);
                    // Get the newly created message from repository to send notification
                    var tmessages = await _messageRepository.GetMessagesForConversationAsync(conversationId, 1, 0);
                    var tlatestMessage = tmessages.FirstOrDefault();

                    if (tlatestMessage != null)
                    {
                        // Notify connected clients about the new message
                        await _chatNotificationService.NotifyMessageSent(conversationId, tlatestMessage);
                    }

                    return "";
                }
                else
                {
                    // Handle voice message translation with multiple target languages
                    var taskIds = await handler.HandleTranslationAsync(message, conversationId, userId, targetLanguages);
                    
                    // Create voice message but DON'T save to database yet - wait for all translations to complete
                    VoiceMessage voiceMessage = new VoiceMessage
                    {
                        Id = Guid.NewGuid(),
                        ConversationId = conversationId,
                        ConversationUserId = message.SenderConversationUserId,
                        VoiceUrl = message.VoiceFileUrl ?? throw new ArgumentException("Voice file URL is required"),
                        DurationSeconds = message.DurationSeconds ?? 0,
                        SentAt = DateTime.UtcNow,
                        IsTranslated = false,
                        TranslatedVoiceUrls = new Dictionary<string, string>(), // Initialize empty dictionary
                        Status = MessageStatus.Sent,
                        ReplyToMessageId = message.ReplyToMessageId // Set reply reference
                    };
                    
                    // Create translation tracking object
                    var translationTracker = new VoiceTranslationTracker
                    {
                        MessageId = voiceMessage.Id,
                        ConversationId = conversationId,
                        TargetLanguages = targetLanguages,
                        TaskIds = taskIds,
                        CompletedTranslations = new Dictionary<string, string>(),
                        CreatedAt = DateTime.UtcNow,
                        VoiceMessage = voiceMessage // Store the message in tracker instead of saving to DB
                    };
                    
                    // Cache the translation tracker for each task ID
                    foreach (var taskId in taskIds)
                    {
                        _memoryCache.Set(taskId, translationTracker, TimeSpan.FromMinutes(30));
                    }
                    
                    // DON'T save message to database and DON'T send initial notification
                    // Message will be saved and notification sent only when all translations are complete
                    
                    _logger.LogInformation($"Voice message translation started. MessageId: {voiceMessage.Id}, TaskIds: [{string.Join(", ", taskIds)}], ConversationId: {conversationId}");
                    
                    return voiceMessage.Id.ToString(); // Return the message ID for tracking
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
    
    // Helper class to track translation progress
    public class VoiceTranslationTracker
    {
        public Guid MessageId { get; set; }
        public Guid ConversationId { get; set; }
        public List<string> TargetLanguages { get; set; } = new List<string>();
        public List<string> TaskIds { get; set; } = new List<string>();
        public Dictionary<string, string> CompletedTranslations { get; set; } = new Dictionary<string, string>();
        public DateTime CreatedAt { get; set; }
        public VoiceMessage VoiceMessage { get; set; } // Store the message until all translations complete
        
        public bool IsComplete => TargetLanguages.All(lang => CompletedTranslations.ContainsKey(lang));
        public int TotalTranslations => TargetLanguages.Count;
        public int CompletedCount => CompletedTranslations.Count;
    }
}
