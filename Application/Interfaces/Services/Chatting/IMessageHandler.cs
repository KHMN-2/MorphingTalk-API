using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Chatting;

namespace Application.Interfaces.Services.Chatting
{
    public interface IMessageHandler
    {
        Task HandleMessageAsync(IMessage message);
        bool CanHandle(MessageType messageType);
    }
}
