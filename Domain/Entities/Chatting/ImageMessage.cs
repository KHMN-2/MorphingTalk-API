using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chatting
{
    public class ImageMessage : Message
    {
        public string ImageUrl { get; set; }
        public MessageType Type => MessageType.Image;
    }
}
