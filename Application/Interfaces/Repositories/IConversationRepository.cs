using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Users;
using Domain.Entities.Chatting;

namespace Application.Interfaces.Repositories
{
    public interface IConversationRepository
    {
        Task<Conversation> GetConversationByIdAsync(Guid chatId);
        Task AddConversationAsync(Conversation chat);
        Task AddMessageAsync(Message message);
        Task AddUserToConversationAsync(Guid chatId, string userId);
        Task RemoveUserFromConversationAsync(Guid chatId, string userId);
        Task<List<User>> GetUsersInConversationAsync(Guid chatId);
    }
}
