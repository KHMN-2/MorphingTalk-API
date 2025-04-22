using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chatting
{
    public class Chat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
        public List<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();
    }
}
