using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Users;

namespace Domain.Entities.Chatting
{
    public class ChatUser
    {
        public int ChatId { get; set; }
        public Chat Chat { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public DateTime JoinedAt { get; set; }
        public string Role { get; set; } // e.g., "Admin", "Member"
    }
}
