using Domain.Entities.Chatting;

namespace Application.DTOs.Chatting
{
    public class MessageDto
    {
        public string Type { get; set; } // "text" or "voice"
        public string Text { get; set; }
        public string VoiceFileUrl { get; set; }
        public double DurationSeconds { get; set; }
    }
    public class MessageSummaryDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } // e.g. "text", "voice"
        public string SenderUserId { get; set; }
        public string SenderDisplayName { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }
    }
    public class SendMessageDto
    {
        public MessageType Type { get; set; } // "text" or "voice"
        public Guid SenderConversationUserId { get; set; }
        public string Text { get; set; } // Required for "text"
        public string VoiceFileUrl { get; set; } // Required for "voice"
        public double? DurationSeconds { get; set; } // Optional for "voice"
    }
}
