using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chatting
{
    public class Message
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }
    }
}
