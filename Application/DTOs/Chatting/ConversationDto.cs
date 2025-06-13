using Application.DTOs.Chatting;
using Domain.Entities.Chatting;
using MorphingTalk_API.DTOs.Chatting;

namespace Application.DTOs.Chatting
{
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }            // For group conversations (optional for direct)
        public string? GroupImageUrl { get; set; }   // Optional for group conversations
        public string? Description { get; set; }    // Optional for group conversations
        public ConversationType Type { get; set; }            // "group" or "direct"
        public DateTime CreatedAt { get; set; }
        public ICollection<ConversationUserDto> Users { get; set; }
        public MessageSummaryDto LastMessage { get; set; }
        // Add more fields as needed (e.g., Conversation Icon, Admin, etc.)
        public ConversationUserDto LoggedInConversationUser { get; set; }
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
        public string? Name { get; set; }
        public string? GroupImageUrl { get; set; }   // Optional for group conversations
        public ConversationType Type { get; set; } // "group" or "direct"
        public List<string> UserEmails { get; set; } // Initial user list (can include self)
    }
}
