using Application.Interfaces.Repositories;
using Domain.Entities.Chatting;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ConversationUserRepository : IConversationUserRepository
{
    private readonly ApplicationDbContext _context;

    public ConversationUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Get ConversationUser by ConversationId and UserId (composite key scenario)
    public async Task<ConversationUser> GetByIdsAsync(Guid conversationId, string userId)
    {
        return await _context.ConversationUsers
            .Include(cu => cu.User)
            .Include(cu => cu.Conversation)
            .FirstOrDefaultAsync(cu => cu.ConversationId == conversationId && cu.UserId == userId);
    }

    // Add user to conversation
    public async Task<ConversationUser> AddAsync(ConversationUser conversationUser)
    {
        _context.ConversationUsers.Add(conversationUser);
        await _context.SaveChangesAsync();
        return conversationUser;
    }

    // Remove user from conversation
    public async Task RemoveAsync(Guid conversationId, string userId)
    {
        var cu = await _context.ConversationUsers
            .FirstOrDefaultAsync(x => x.ConversationId == conversationId && x.UserId == userId);
        if (cu != null)
        {
            _context.ConversationUsers.Remove(cu);
            await _context.SaveChangesAsync();
        }
    }

    // Get all users for a conversation
    public async Task<List<ConversationUser>> GetUsersForConversationAsync(Guid conversationId)
    {
        return await _context.ConversationUsers
            .Where(cu => cu.ConversationId == conversationId)
            .Include(cu => cu.User)
            .ToListAsync();
    }

    // Get all conversations for a user
    public async Task<List<ConversationUser>> GetConversationsForUserAsync(string userId)
    {
        return await _context.ConversationUsers
            .Where(cu => cu.UserId == userId)
            .Include(cu => cu.Conversation)
            .ToListAsync();
    }
}