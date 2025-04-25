using Application.DTOs.Chatting;
using Domain.Entities.Chatting;

namespace MorphingTalk_API.DTOs.Chatting
{
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }            // For group conversations (optional for direct)
        public ConversationType Type { get; set; }            // "group" or "direct"
        public DateTime CreatedAt { get; set; }
        public ICollection<ConversationUserDto> Users { get; set; }
        public MessageSummaryDto LastMessage { get; set; }
        // Add more fields as needed (e.g., Conversation Icon, Admin, etc.)
    }
    public class ConversationListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<ConversationUserDto> Users { get; set; }
        public MessageSummaryDto LastMessage { get; set; }
    }

    public class CreateConversationDto
    {
        public string Name { get; set; }
        public ConversationType Type { get; set; } // "group" or "direct"
        public List<string> UserEmails { get; set; } // Initial user list (can include self)
    }
}
