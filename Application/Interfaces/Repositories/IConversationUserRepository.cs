using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Chatting;

namespace Application.Interfaces.Repositories
{
    public interface IConversationUserRepository
    {
        public Task AddConverstaionUserAsync(ConversationUser conversationUser);
    }
}
