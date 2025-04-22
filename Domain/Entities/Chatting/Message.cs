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
        public string SenderId { get; set; }
        public DateTime Timestamp { get; set; }
        public MessageType Type { get; }
        public  MessageStatus Status { get; set; }
        public Guid ConversationId { get; set; }

    }
}
