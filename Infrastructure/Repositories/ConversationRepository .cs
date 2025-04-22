using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Domain.Entities.Chatting;
using Domain.Entities.Users;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly IdentityDbContext _context;

        public ConversationRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation> GetConversationByIdAsync(Guid chatId)
        {
            return await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == chatId);
        }

        public async Task AddConversationAsync(Conversation chat)
        {
            await _context.Conversations.AddAsync(chat);
            await _context.SaveChangesAsync();
        }

        public async Task AddMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task AddUserToConversationAsync(Guid chatId, string userId)
        {
            var chatUser = new ConversationUser { ConversationId = chatId, UserId = userId };
            await _context.ConversationUsers.AddAsync(chatUser);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserFromConversationAsync(Guid chatId, string userId)
        {
            var chatUser = await _context.ConversationUsers
                .FirstOrDefaultAsync(cu => cu.ConversationId == chatId && cu.UserId == userId);

            if (chatUser != null)
            {
                _context.ConversationUsers.Remove(chatUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<User>> GetUsersInConversationAsync(Guid chatId)
        {
            return await _context.ConversationUsers
                .Where(cu => cu.ConversationId == chatId)
                .Select(cu => cu.User)
                .ToListAsync();
        }
    }
}
