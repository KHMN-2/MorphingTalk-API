using Application.DTOs.Chatting;
using Domain.Entities.Chatting;
using Domain.Entities.Users;
using MorphingTalk_API.DTOs.Chatting;
using System.ComponentModel.DataAnnotations;

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
        public MessageSummaryDto LastMessage { get; set; }  // Add more fields as needed (e.g., Conversation Icon, Admin, etc.)
        public ConversationUserDto LoggedInConversationUser { get; set; }
        public bool muteNotifications { get; set; } = false; // Optional, if needed
        public bool UseRobotVoice { get; set; } = true; // Optional, if needed
        public bool TranslateMessages { get; set; } = false; // Optional, if needed
        public bool IsOtherUserBlocked { get; set; } = false; // Only relevant for one-to-one conversations

        public static ConversationDto fromCoversation(Conversation conversation, string userId)
        {
            var name = conversation.Name;
            if (conversation.Type == ConversationType.OneToOne && conversation.ConversationUsers.Count == 2)
            {
                var otherUser = conversation.ConversationUsers.First(cu => cu.UserId != userId);
                name = otherUser.User?.FullName;
            }
            var loggedInConversationUser = conversation.ConversationUsers?
                .FirstOrDefault(cu => cu.UserId == userId);
            var users = conversation.ConversationUsers?
                .Where(cu => cu.UserId != userId)
                .Select(cu => ConversationUserDto.FromConversationUser(cu))
                .ToList();
            users.Insert(0, ConversationUserDto.FromConversationUser(loggedInConversationUser));
            return new ConversationDto
            {
                Id = conversation.Id,
                Name = name,
                Type = conversation.Type,
                CreatedAt = conversation.CreatedAt,
                GroupImageUrl = conversation.GroupImageUrl,
                Description = conversation.Description,
                muteNotifications = loggedInConversationUser?.muteNotifications ?? false,
                UseRobotVoice = loggedInConversationUser?.UseRobotVoice ?? true,
                TranslateMessages = loggedInConversationUser?.TranslateMessages ?? false,
                LoggedInConversationUser = loggedInConversationUser != null
                    ? ConversationUserDto.FromConversationUser(loggedInConversationUser)
                    : null,

                Users = users ?? new List<ConversationUserDto>(),
                LastMessage = conversation.Messages?
                    .OrderByDescending(m => m.SentAt)
                    .Take(1)
                    .Select(m => MessageSummaryDto.FromMessage(m)).FirstOrDefault(),
            };
        }
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
        public string? Description { get; set; } // Optional for group conversations

    }

    public class UpdateConversationDto
    {
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string? Name { get; set; }
        
        [Url(ErrorMessage = "Group image URL must be a valid URL")]
        public string? GroupImageUrl { get; set; }   // Optional for group conversations
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; } // Optional for group conversations

        // User-specific conversation settings
        public bool? UseRobotVoice { get; set; }
        public bool? TranslateMessages { get; set; }
        public bool? MuteNotifications { get; set; }
    }
}
