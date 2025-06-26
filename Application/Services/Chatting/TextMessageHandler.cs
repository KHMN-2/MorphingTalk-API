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
    public class TextMessageHandler : IMessageHandler
    {
        ILogger<TextMessageHandler> _logger;
        IMessageRepository _messageRepository;
        public TextMessageHandler(ILogger<TextMessageHandler> logger, IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
            
            _logger = logger;
        }

        public bool CanHandle(MessageType messageType) => messageType == MessageType.Text;

        public async Task HandleMessageAsync(SendMessageDto message, Guid conversationId, string userId)
        {
            if (string.IsNullOrEmpty(message.Text))
                throw new ArgumentException("Text content is required for text messages");

            var textMessage = new TextMessage
            {
                Content = message.Text,
                SentAt = DateTime.UtcNow,
                ConversationUserId = message.SenderConversationUserId,
                ConversationId = conversationId,
                Status = MessageStatus.Sent,
            };

            // Save the message to the repository
            await _messageRepository.AddAsync(textMessage);

            // Add text-specific processing logic here
            _logger.LogInformation($"Processing text message: {textMessage.Content}");
            await Task.CompletedTask;
        }

        public async Task<string> HandleTranslationAsync(SendMessageDto message, Guid conversationId, string userId)
        {
            // For text messages, we process the translation directly
            // and don't need to return a task ID since translation is immediate
            
            if (string.IsNullOrEmpty(message.Text))
                throw new ArgumentException("Text content is required for text messages");
            
            // Create the text message with the original text
            var textMessage = new TextMessage
            {
                Content = message.Text,
                SentAt = DateTime.UtcNow,
                ConversationUserId = message.SenderConversationUserId,
                ConversationId = conversationId,
                Status = MessageStatus.Sent,
            };

            // Save the message to the repository
            await _messageRepository.AddAsync(textMessage);

            _logger.LogInformation($"Processing text message with translation: {textMessage.Content}");
            
            // For text messages, we return empty string since translation is handled immediately
            // and doesn't require a task ID
            return string.Empty;
        }
    }
}
