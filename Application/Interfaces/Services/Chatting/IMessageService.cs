using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Chatting;

namespace Application.Interfaces.Services.Chatting
{
    public interface IMessageService
    {
        Task ProcessMessageAsync(Message message);

    }
}
