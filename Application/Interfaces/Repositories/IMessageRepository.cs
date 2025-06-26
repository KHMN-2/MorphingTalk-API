using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Chatting;

namespace Application.Interfaces.Repositories
{
    public interface IMessageRepository
    {
        Task<Message> GetByIdAsync(Guid id);
        Task<List<Message>> GetMessagesForConversationAsync(Guid conversationId, int count = 50, int skip = 0);
        Task<List<Message>> GetMessagesForUserInConversationAsync(Guid conversationId, string userId, int count = 50, int skip = 0);
        Task<Message> AddAsync(Message message);
        Task<Message> UpdateAsync(Message message);
        Task DeleteAsync(Guid id);
    }
}
