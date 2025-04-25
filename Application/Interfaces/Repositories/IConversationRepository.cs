using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Users;
using Domain.Entities.Chatting;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.Repositories
{
    public interface IConversationRepository
    {
        public  Task<Conversation> GetByIdAsync(Guid id);

        // Get all Conversations for a user
        public  Task<List<Conversation>> GetConversationsForUserAsync(string userId);

        // Create a new conversation
        public  Task<Conversation> AddAsync(Conversation conversation);

        // Update a conversation
        public  Task UpdateAsync(Conversation conversation);

        // Delete a conversation
        public  Task DeleteAsync(Guid id);
    }
}
