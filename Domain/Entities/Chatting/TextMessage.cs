using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chatting
{
   
    public class TextMessage : Message
    {
        public string Content { get; set; }
        public MessageType Type => MessageType.Text;

    }
}
