using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Users
{
    public class Friendship
    {
        public Guid Id { get; set; }
        required public string UserId1 { get; set; }
        required public string UserId2
        {
            get; set;
        }
        public DateTime UpdatedAt { get; set; }
        public bool IsBlocked { get; set; }
        public string? BlockedByUserId { get; set; } // Tracks who initiated the block
    
        public User User1 { get; set; }
        public User User2 { get; set; }

    }
}
