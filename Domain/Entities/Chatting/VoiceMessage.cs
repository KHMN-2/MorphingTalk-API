using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chatting
{
    public class VoiceMessage : Message
    {
        
        public string VoiceUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public MessageType Type => MessageType.Voice;
    }
}
