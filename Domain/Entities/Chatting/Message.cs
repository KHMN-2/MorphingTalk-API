using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chatting
{
    public enum MessageStatus
    {
        Sent,
        Delivered,
        Read
    }
    public abstract class Message
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Guid ConversationUserId { get; set; }
        public DateTime SentAt { get; set; }
        public MessageStatus Status { get; set; }
        public virtual Conversation Conversation { get; set; }
        public virtual ConversationUser ConversationUser { get; set; }
    }
}
