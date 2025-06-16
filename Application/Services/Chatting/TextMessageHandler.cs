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
            var textMessage = new TextMessage
            {
                Content = message.Text,
                SentAt = DateTime.UtcNow,
                ConversationUserId = message.SenderConversationUserId,
                ConversationId = conversationId,
                Status = MessageStatus.Sent,
            };

            if (textMessage == null) throw new InvalidOperationException("Invalid message type");

            // Save the message to the repository
            await _messageRepository.AddAsync(textMessage);



            // Add text-specific processing logic here
            _logger.LogInformation($"Processing text message: {textMessage.Content}");
            await Task.CompletedTask;
        }

        public Task<string> HandleTranslationAsync(SendMessageDto message, Guid conversationId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
