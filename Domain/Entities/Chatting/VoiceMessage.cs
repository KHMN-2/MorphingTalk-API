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
        public bool IsTranslated { get; set; }
        public string? TranslatedVoiceUrl { get; set; }
        public int? DurationSeconds { get; set; }
        public MessageType Type => MessageType.Voice;
    }
}
