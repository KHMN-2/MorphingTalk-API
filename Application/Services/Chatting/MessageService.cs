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


        public MessageService(IEnumerable<IMessageHandler> handlers, IConversationUserRepository conversationUserRepository, IMessageRepository messageRepository, IChatNotificationService chatNotificationService, IMemoryCache memoryCache)
        {
            _handlers = handlers;
            _conversationUserRepo = conversationUserRepository;
            _messageRepository = messageRepository;
            _chatNotificationService = chatNotificationService;
            _memoryCache = memoryCache;
        }

        public async Task<bool> ProcessMessageAsync(SendMessageDto message, Guid conversationId, string userId)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(message.Type));
            var cu = await _conversationUserRepo.GetByIdsAsync(conversationId, userId);
            if (cu == null)
                throw new KeyNotFoundException("ConversationUser not found");
            if (handler == null)
            {
                _logger.LogError($"No handler found for message type: {message.Type}");
                throw new NotSupportedException($"Message type {message.Type} is not supported");
            }
            if (message.NeedTranslation)
            {
                var cacheKey = $"{conversationId}_" + Guid.NewGuid();
              
                _memoryCache.Set(cacheKey, message, TimeSpan.FromMinutes(30));
                message.SenderConversationUserId = cu.Id;

                return false; // Indicate that the message is being processed for translation
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

            return true;
        }
    }
}
