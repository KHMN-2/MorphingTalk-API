using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Users;
using Domain.Entities.Chatting;

namespace Application.Interfaces.Repositories
{
    public interface IChatRepository
    {
        Task<Chat> GetChatByIdAsync(int chatId);
        Task AddChatAsync(Chat chat);
        Task AddMessageAsync(Message message);
        Task AddUserToChatAsync(int chatId, string userId);
        Task RemoveUserFromChatAsync(int chatId, string userId);
        Task<List<User>> GetUsersInChatAsync(int chatId);
    }
}
