using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Chatting;

namespace Application.DTOs.Chatting
{
    public class ConversationUserDto
    {
        public Guid? ConversationUserId { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; } // Optional, if needed
        public string DisplayName { get; set; }
        public bool IsOnline { get; set; } // Optional, if needed
        public DateTime LastSeenAt { get; set; } // Optional, if needed
        public string ProfileImagePath { get; set; } // Optional, if needed
        public String Role { get; set; } // e.g., "Admin", "Member"
        public string bio { get; set; } // Optional, if needed\
        public DateTime LastUpdate { get; set; }

        public static ConversationUserDto FromConversationUser(ConversationUser conversationUser)
        {
            return new ConversationUserDto
            {
                ConversationUserId = conversationUser.Id,
                UserId = conversationUser.UserId,
                Email = conversationUser.User?.Email ?? string.Empty, // Optional, if needed
                DisplayName = conversationUser.User?.FullName,
                Role = conversationUser.Role.ToString(),
                ProfileImagePath = conversationUser.User?.ProfilePicturePath,
                bio = conversationUser.User?.AboutStatus,
                IsOnline = conversationUser.User?.IsOnline ?? false,
                LastSeenAt = conversationUser.User?.LastSeen ?? DateTime.MinValue,
                LastUpdate = conversationUser.User?.LastUpdatedOn ?? DateTime.MinValue
            };
        }

    }

    public class AddUserToConversationDto
    {
        public string Email { get; set; }
    }
}
