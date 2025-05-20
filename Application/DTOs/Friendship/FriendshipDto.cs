using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Friendship
{
    public class FriendshipDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; } // Optional, if needed
        public DateTime LastSeen { get; set; } // Optional, if needed
        public string ProfileImagePath { get; set; } // Optional, if needed
    }
}
