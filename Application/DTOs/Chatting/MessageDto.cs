using Domain.Entities.Chatting;
using System;
using System.Collections.Generic;

namespace Application.DTOs.Chatting
{
    public class MessageDto
    {
        public string Type { get; set; } // "text", "voice", or "image"
        public string? Text { get; set; }
        public string? VoiceFileUrl { get; set; }
        public int? DurationSeconds { get; set; }
        public string? FileUrl { get; set; } // For image or other file types
        public string? ImageUrl { get; set; } // For image messages
        public string? Caption { get; set; } // For image captions
        public string? FileName { get; set; } // For file names
        public long? FileSizeBytes { get; set; } // For file sizes
    }
    public class MessageSummaryDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } // "Text", "Voice", or "Image"
        public string? SenderId { get; set; }
        public string? SenderDisplayName { get; set; }
        public string? Text { get; set; } // For text messages
        public Dictionary<string, string>? TranslatedTexts { get; set; } // For text messages (language -> text mapping)
        public string? VoiceFileUrl { get; set; } // For voice messages (original voice URL)
        public Dictionary<string, string>? TranslatedVoiceUrls { get; set; } // For voice messages (language -> URL mapping)
        public int? Duration { get; set; } // For voice messages
        public bool IsTranslated { get; set; } // For both text and voice messages
        public string? ImageUrl { get; set; } // For image messages
        public DateTime SentAt { get; set; }
        public string MessageStatus { get; set; }
        public string ConversationId { get; set; }

        public static MessageSummaryDto FromMessage(Message message)
        {
            return new MessageSummaryDto
            {
                Id = message.Id,
                Type = message is TextMessage ? MessageType.Text.ToString() :
                       message is VoiceMessage ? MessageType.Voice.ToString() :
                       message is ImageMessage ? MessageType.Image.ToString() : "unknown",
                SenderId = message.ConversationUser?.UserId,
                SenderDisplayName = message.ConversationUser?.User?.FullName,
                Text = message is TextMessage tm ? tm.Content : null,
                TranslatedTexts = message is TextMessage textMsg ? textMsg.TranslatedContents : null,
                VoiceFileUrl = message is VoiceMessage vm ? vm.VoiceUrl : null, // Always use original URL
                TranslatedVoiceUrls = message is VoiceMessage voiceMsg ? voiceMsg.TranslatedVoiceUrls : null,
                Duration = message is VoiceMessage v ? v.DurationSeconds : null,
                IsTranslated = (message is VoiceMessage voiceMessage && voiceMessage.IsTranslated) || 
                              (message is TextMessage textMessage && textMessage.IsTranslated),
                ImageUrl = message is ImageMessage im ? im.ImageUrl : null,
                SentAt = message.SentAt,
                MessageStatus = message.Status.ToString(),
                ConversationId = message.ConversationId.ToString()
            };
        }

    }
    public class SendMessageDto
    {
        public MessageType Type { get; set; } // "text", "voice", or "image"
        public Guid SenderConversationUserId { get; set; }
        public string? Text { get; set; } // Required for "text"
        public string? VoiceFileUrl { get; set; } // Required for "voice"
        public int? DurationSeconds { get; set; } // Optional for "voice"
        public string? ImageUrl { get; set; } // Required for "image"
        public bool? UseRobotVoice { get; set; }
        public bool NeedTranslation { get; set; }
    }
}
