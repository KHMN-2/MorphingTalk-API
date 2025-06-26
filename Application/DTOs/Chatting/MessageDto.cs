using Domain.Entities.Chatting;

namespace Application.DTOs.Chatting
{
    public class MessageDto
    {
        public string Type { get; set; } // "text" or "voice"
        public string? Text { get; set; }
        public string? VoiceFileUrl { get; set; }
        public int? DurationSeconds { get; set; }
    }
    public class MessageSummaryDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } // e.g. "text", "voice"
        public string SenderId { get; set; }
        public string SenderDisplayName { get; set; }
        public string? Text { get; set; }
        public string? VoiceFileUrl { get; set; }
        public int? Duration { get; set; }
        public bool IsTranslated { get; set; } // Indicates if the message has been translated
        public string? OriginalVoiceFileUrl { get; set; }
        public DateTime SentAt { get; set; }
        public string MessageStatus { get; set; } // e.g. "sent", "delivered", "read"
        public string ConversationId { get; set; } // Optional, if needed

        public static MessageSummaryDto FromMessage(Message message)
        {
            return new MessageSummaryDto
            {
                Id = message.Id,
                Type = message is TextMessage ? MessageType.Text.ToString() :
                       message is VoiceMessage ? MessageType.Voice.ToString() : "unknown",
                SenderId = message.ConversationUser?.UserId,
                SenderDisplayName = message.ConversationUser?.User?.FullName,
                Text = message is TextMessage tm ? tm.Content : null,
                VoiceFileUrl = message is VoiceMessage vm ? (vm.IsTranslated ? vm.TranslatedVoiceUrl : vm.VoiceUrl) : null,
                Duration = message is VoiceMessage v ? v.DurationSeconds : null,
                IsTranslated = message is VoiceMessage voiceMsg && voiceMsg.IsTranslated,
                OriginalVoiceFileUrl = message is VoiceMessage originalVm ? originalVm.VoiceUrl : null,
                SentAt = message.SentAt,
                MessageStatus = message.Status.ToString(),
                ConversationId = message.ConversationId.ToString()
            };

        }

    }
    public class SendMessageDto
    {
        public MessageType Type { get; set; } // "text" or "voice"
        public Guid SenderConversationUserId { get; set; }
        public string? Text { get; set; } // Required for "text"
        public string? VoiceFileUrl { get; set; } // Required for "voice"
        public int? DurationSeconds { get; set; } // Optional for "voice"
        public bool? UseRobotVoice { get; set; }
        public bool NeedTranslation { get; set; }
        public string? TargetLanguage { get; set; }
    }
}
