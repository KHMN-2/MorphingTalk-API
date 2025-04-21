using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chatting
{
    public class VoiceMessage : IMessage
    {
        public Guid Id { get; set; }
        public string SenderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string VoiceUrl { get; set; }
        public int Duration { get; set; }
        public MessageType Type => MessageType.Voice;
    }
}
