using Application.Interfaces.Repositories;
using Domain.Entities.Chatting;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ConversationRepository : IConversationRepository
{
    private readonly ApplicationDbContext _context;

    public ConversationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Get Conversation by Id (with participants)
    public async Task<Conversation> GetByIdAsync(Guid id)
    {
        return await _context.Conversations
            .Include(c => c.ConversationUsers)
                .ThenInclude(cu => cu.User)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    // Get all Conversations for a user
    public async Task<List<Conversation>> GetConversationsForUserAsync(string userId)
    {
        // include Last Message
        return await _context.ConversationUsers
            .Where(cu => cu.UserId == userId)
            .Include(cu => cu.Conversation)
                .ThenInclude(c => c.Messages)
                    .ThenInclude(m => m.ConversationUser)
            .Include(cu => cu.Conversation)
                .ThenInclude(c => c.ConversationUsers)
                    .ThenInclude(cu2 => cu2.User)
            .Select(cu => cu.Conversation)
            .ToListAsync();
    }

    public async Task<Conversation> GetOneToOneConversationAsync(string userId1, string userId2)
    {
        return await _context.Conversations
            .Include(c => c.ConversationUsers)
                .ThenInclude(cu => cu.User)
            .FirstOrDefaultAsync(c => c.Type == ConversationType.OneToOne &&
                                      c.ConversationUsers.Any(cu => cu.UserId == userId1) &&
                                      c.ConversationUsers.Any(cu => cu.UserId == userId2));
    
    }

    // Create a new conversation
    public async Task<Conversation> AddAsync(Conversation conversation)
    {
        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();
        return conversation;
    }

    // Update a conversation
    public async Task UpdateAsync(Conversation conversation)
    {
        _context.Conversations.Update(conversation);
        await _context.SaveChangesAsync();
    }

    // Delete a conversation
    public async Task DeleteAsync(Guid id)
    {
        var conversation = await _context.Conversations.FindAsync(id);
        if (conversation != null)
        {
            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();
        }
    }
}