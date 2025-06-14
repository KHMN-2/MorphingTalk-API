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
    public class VoiceMessageHandler : IMessageHandler
    {
        private readonly ILogger<VoiceMessageHandler> _logger;
        private readonly IMessageRepository _messageRepository;

        public VoiceMessageHandler(ILogger<VoiceMessageHandler> logger, IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }

        public bool CanHandle(MessageType messageType) => messageType == MessageType.Voice;

        public async Task HandleMessageAsync(SendMessageDto message, Guid conversationId, string userId)
        {
            var voiceMessage = new VoiceMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                ConversationUserId = message.SenderConversationUserId,
                VoiceUrl = message.VoiceFileUrl, // Assuming SendMessageDto has this property
                VoiceDuration = (double) message.DurationSeconds, // Assuming SendMessageDto has this property
                SentAt = DateTime.UtcNow,

            };
            if (voiceMessage == null) throw new InvalidOperationException("Invalid message type");

            // Save the message to the repository
            await _messageRepository.AddAsync(voiceMessage);


            // Add text-specific processing logic here
            _logger.LogInformation($"Processing voice message: {voiceMessage.VoiceUrl} with duration {voiceMessage.VoiceDuration} seconds");
            await Task.CompletedTask;
        }

    }
}
