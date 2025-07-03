using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chatting;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Microsoft.Extensions.Logging;
using MorphingTalk_API.DTOs.Chatting;

namespace Application.Services.Chatting
{
    public class ImageMessageHandler : IMessageHandler
    {
        private readonly ILogger<ImageMessageHandler> _logger;
        private readonly IMessageRepository _messageRepository;

        public ImageMessageHandler(ILogger<ImageMessageHandler> logger, IMessageRepository messageRepository)
        {
            _logger = logger;
            _messageRepository = messageRepository;
        }

        public bool CanHandle(MessageType messageType) => messageType == MessageType.Image;

        public async Task HandleMessageAsync(SendMessageDto message, Guid conversationId, string userId)
        {
            if (string.IsNullOrEmpty(message.ImageUrl))
                throw new ArgumentException("Image URL is required for image messages");

            var imageMessage = new ImageMessage
            {
                ImageUrl = message.ImageUrl,
                SentAt = DateTime.UtcNow,
                ConversationUserId = message.SenderConversationUserId,
                ConversationId = conversationId,
                Status = MessageStatus.Sent,
                ReplyToMessageId = message.ReplyToMessageId
            };

            // Save the message to the repository
            await _messageRepository.AddAsync(imageMessage);

            _logger.LogInformation($"Processing image message: {imageMessage.ImageUrl}");
            await Task.CompletedTask;
        }

        public async Task<List<string>> HandleTranslationAsync(SendMessageDto message, Guid conversationId, string userId, List<string> targetLanguages)
        {
            // For image messages, translation is not applicable
            // We just process it as a regular image message
            await HandleMessageAsync(message, conversationId, userId);
            
            _logger.LogInformation($"Image message processed (no translation needed): {message.ImageUrl}");
            
            // Return empty list since image messages don't require translation processing
            return new List<string>();
        }
    }
} 