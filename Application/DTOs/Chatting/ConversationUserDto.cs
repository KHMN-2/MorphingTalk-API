using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Chatting
{
    public class ConversationUserDto
    {
        public Guid? ConversationUserId { get; set; }
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public bool IsOnline { get; set; } // Optional, if needed
        public DateTime LastSeenAt { get; set; } // Optional, if needed
        public string ProfileImagePath { get; set; } // Optional, if needed


    }

    public class AddUserToConversationDto
    {
        public string UserId { get; set; }
    }
}
