using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Users;

namespace Domain.Entities.Chatting
{
    public enum Roles
    {
        Admin,
        Member,
    }
    public class ConversationUser
    {
        public Guid Id { get; set; } 
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime JoinedAt { get; set; }
        public Roles Role { get; set; } // e.g., "Admin", "Member"
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public ICollection<Message> Messages { get; set; }
        public bool LeftConversation { get; set; } = false;
    }
}
