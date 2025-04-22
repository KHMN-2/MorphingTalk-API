using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Chatting
{
    public interface IMessage
    {
        Guid Id { get; set; }
        string SenderId { get; set; }
        DateTime Timestamp { get; set; }
        MessageType Type { get; }
    }
}
