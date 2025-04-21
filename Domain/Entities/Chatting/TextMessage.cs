using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chatting
{
    public class TextMessage : IMessage
    {
        public Guid Id { get; set; }
        public string SenderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Content { get; set; }
        public MessageType Type => MessageType.Text;
    }
}
