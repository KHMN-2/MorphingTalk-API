using Application.DTOs;
using Application.DTOs.Chatting;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Chatting;
using Domain.Entities.Chatting;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Chatting
{
    public class AIWebhookService : IAIWebhookService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IChatNotificationService _chatNotificationService;

        public AIWebhookService(
            IMessageRepository messageRepository,
            IChatNotificationService chatNotificationService)
        {
            _messageRepository = messageRepository;
            _chatNotificationService = chatNotificationService;
        }

        public async Task<ResponseViewModel<string>> HandleTextTranslationWebhookAsync(AIWebhookPayloadDto payload)
        {
            if (!payload.Success)
            {
                return new ResponseViewModel<string>(null, $"Translation failed: {payload.ErrorMessage}", false, StatusCodes.Status400BadRequest);
            }

            try
            {
                //var messageId = Guid.Parse(payload.TextTranslation.MessageId);
                //var message = await _messageRepository.GetByIdAsync(messageId);

                //if (message == null)
                //{
                //    return new ResponseViewModel<string>(null, "Message not found", false, StatusCodes.Status404NotFound);
                //}

                //if (message is TextMessage textMessage)
                //{
                //    // Create a new translated message
                //    var translatedMessage = new TextMessage
                //    {
                //        Content = payload.TextTranslation.TranslatedText,
                //        ConversationId = message.ConversationId,
                //        ConversationUserId = message.ConversationUserId,
                //        SentAt = DateTime.UtcNow,
                //        Status = MessageStatus.Sent,
                //        IsTranslated = true,
                //        OriginalMessageId = messageId,
                //        Language = payload.TextTranslation.TargetLanguage
                //    };

                //    await _messageRepository.AddAsync(translatedMessage);

                //    // Notify all users about the translation
                //    await _chatNotificationService.NotifyMessageTranslated(
                //        message.ConversationId,
                //        translatedMessage.Id,
                //        message.ConversationUser.UserId,
                //        payload.TextTranslation.TargetLanguage
                //    );
                //    return new ResponseViewModel<string>(translatedMessage.Id.ToString(), "Translation processed successfully", true, StatusCodes.Status200OK);
                //}

                return new ResponseViewModel<string>(null, "Message is not a text message", false, StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                return new ResponseViewModel<string>(null, $"Error processing translation: {ex.Message}", false, StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResponseViewModel<string>> HandleVoiceTranslationWebhookAsync(AIWebhookPayloadDto payload)
        {
            if (!payload.Success)
            {
                return new ResponseViewModel<string>(null, $"Voice translation failed: {payload.ErrorMessage}", false, StatusCodes.Status400BadRequest);
            }

            try
            {
                //var messageId = Guid.Parse(payload.VoiceTranslation.MessageId);
                //var message = await _messageRepository.GetByIdAsync(messageId);
                
                //if (message == null)
                //{
                //    return new ResponseViewModel<string>(null, "Message not found", false, StatusCodes.Status404NotFound);
                //}

                //if (message is VoiceMessage voiceMessage)
                //{
                //    // Create a new translated voice message
                //    var translatedMessage = new VoiceMessage
                //    {
                //        VoiceUrl = payload.VoiceTranslation.TranslatedVoiceUrl,
                //        VoiceDuration = payload.VoiceTranslation.Duration,
                //        ConversationId = message.ConversationId,
                //        ConversationUserId = message.ConversationUserId,
                //        SentAt = DateTime.UtcNow,
                //        Status = MessageStatus.Sent,
                //        IsTranslated = true,
                //        OriginalMessageId = messageId,
                //        Language = payload.VoiceTranslation.TargetLanguage
                //    };

                //    await _messageRepository.AddAsync(translatedMessage);

                //    // Notify all users about the translation
                //    await _chatNotificationService.NotifyMessageTranslated(
                //        message.ConversationId,
                //        translatedMessage.Id,
                //        message.ConversationUser.UserId,
                //        payload.VoiceTranslation.TargetLanguage
                //    );

                //    return new ResponseViewModel<string>(translatedMessage.Id.ToString(), "Voice translation processed successfully", true, StatusCodes.Status200OK);
                //}

                return new ResponseViewModel<string>(null, "Message is not a voice message", false, StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                return new ResponseViewModel<string>(null, $"Error processing voice translation: {ex.Message}", false, StatusCodes.Status500InternalServerError);
            }
        }
    }
} 