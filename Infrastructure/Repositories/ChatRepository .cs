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
    public class ChatRepository : IChatRepository
    {
        private readonly IdentityDbContext _context;

        public ChatRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<Chat> GetChatByIdAsync(int chatId)
        {
            return await _context.Chats
                .Include(c => c.Messages)
                .Include(c => c.ChatUsers)
                .ThenInclude(cu => cu.User)
                .FirstOrDefaultAsync(c => c.Id == chatId);
        }

        public async Task AddChatAsync(Chat chat)
        {
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();
        }

        public async Task AddMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task AddUserToChatAsync(int chatId, string userId)
        {
            var chatUser = new ChatUser { ChatId = chatId, UserId = userId };
            await _context.ChatUsers.AddAsync(chatUser);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserFromChatAsync(int chatId, string userId)
        {
            var chatUser = await _context.ChatUsers
                .FirstOrDefaultAsync(cu => cu.ChatId == chatId && cu.UserId == userId);

            if (chatUser != null)
            {
                _context.ChatUsers.Remove(chatUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<User>> GetUsersInChatAsync(int chatId)
        {
            return await _context.ChatUsers
                .Where(cu => cu.ChatId == chatId)
                .Select(cu => cu.User)
                .ToListAsync();
        }
    }
}
