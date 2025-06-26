using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Users;

namespace Domain.Entities.Chatting
{
    public enum ConversationType
    {
        OneToOne,
        Group
    }
    public class Conversation
    {
        public Guid Id { get; set; }
        public ConversationType Type { get; set; }
        public string? Name { get; set; }
        public string? GroupImageUrl { get; set; } // Optional for group conversations
        public string? Description { get; set; } // Optional for group conversations
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public ICollection<ConversationUser> ConversationUsers { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
