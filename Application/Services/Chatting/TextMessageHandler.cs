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
        private readonly ILogger<TextMessageHandler> _logger;
        private readonly IMessageRepository _messageRepository;
        private readonly ITextTranslationService _textTranslationService;
        private readonly IUserRepository _userRepository;

        public TextMessageHandler(
            ILogger<TextMessageHandler> logger, 
            IMessageRepository messageRepository,
            ITextTranslationService textTranslationService,
            IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _textTranslationService = textTranslationService;
            _userRepository = userRepository;
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
                IsTranslated = false,
                TranslatedContents = new Dictionary<string, string>(),
                ReplyToMessageId = message.ReplyToMessageId
            };

            // Save the message to the repository
            await _messageRepository.AddAsync(textMessage);

            // Add text-specific processing logic here
            _logger.LogInformation($"Processing text message: {textMessage.Content}");
            await Task.CompletedTask;
        }

        public async Task<List<string>> HandleTranslationAsync(SendMessageDto message, Guid conversationId, string userId, List<string> targetLanguages)
        {
            if (string.IsNullOrEmpty(message.Text))
                throw new ArgumentException("Text content is required for text messages");

            // Get the user to determine source language
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            string sourceLanguage = user.NativeLanguage ?? "en";
            
            _logger.LogInformation($"Starting text translation from {sourceLanguage} to [{string.Join(", ", targetLanguages)}]");

            // Perform translation
            var translations = await _textTranslationService.TranslateTextAsync(message.Text, sourceLanguage, targetLanguages);

            // Create the text message with translations
            var textMessage = new TextMessage
            {
                Content = message.Text,
                SentAt = DateTime.UtcNow,
                ConversationUserId = message.SenderConversationUserId,
                ConversationId = conversationId,
                Status = MessageStatus.Sent,
                IsTranslated = translations.Any(),
                TranslatedContents = translations,
                TranslatedContent = translations.Values.FirstOrDefault(),
                ReplyToMessageId = message.ReplyToMessageId
            };

            // Save the message to the repository
            await _messageRepository.AddAsync(textMessage);

            _logger.LogInformation($"Text message translation completed. Original: '{message.Text}', Translations: {translations.Count}");
            
            // For text messages, we return empty list since translation is handled immediately
            // and doesn't require task IDs
            return new List<string>();
        }
    }
}
