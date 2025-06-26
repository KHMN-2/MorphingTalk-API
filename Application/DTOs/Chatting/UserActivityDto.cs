using System;

namespace Application.DTOs.Chatting
{
    public class UserStatusDto
    {
        public string UserId { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime? LastSeen { get; set; }
    }

    public class TypingIndicatorDto
    {
        public string UserId { get; set; } = string.Empty;
        public string ConversationId { get; set; } = string.Empty;
        public bool IsTyping { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class OnlineStatusRequestDto
    {
        public string[] UserIds { get; set; } = Array.Empty<string>();
    }

    public class OnlineStatusResponseDto
    {
        public Dictionary<string, UserStatusDto> UserStatuses { get; set; } = new();
        public DateTime RequestTimestamp { get; set; }
    }
}
