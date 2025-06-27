using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Chatting;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.Repositories
{
    public interface IConversationUserRepository
    {
        // Get ConversationUser by ConversationId and UserId (composite key scenario)
        public Task<ConversationUser> GetByIdsAsync(Guid conversationId, string userId);

        // Add user to conversation
        public Task<ConversationUser> AddAsync(ConversationUser conversationUser);

        // Update user conversation settings
        public Task<ConversationUser> UpdateAsync(ConversationUser conversationUser);

        // Remove user from conversation
        public Task RemoveAsync(Guid conversationId, string userId);

        // Get all users for a conversation
        public Task<List<ConversationUser>> GetUsersForConversationAsync(Guid conversationId);

        // Get all conversations for a user
        public Task<List<ConversationUser>> GetConversationsForUserAsync(string userId);
    }
}
