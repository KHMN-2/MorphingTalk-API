using Application.Interfaces.Repositories;
using Domain.Entities.Chatting;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Message> GetByIdAsync(Guid id)
    {
        return await _context.Messages
            .Include(m => m.ConversationUser)
                .ThenInclude(cu => cu.User)
            .Include(m => m.Conversation)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Message> GetByIdWithReplyAsync(Guid id)
    {
        return await _context.Messages
            .Include(m => m.ConversationUser)
                .ThenInclude(cu => cu.User)
            .Include(m => m.Conversation)
            .Include(m => m.ReplyToMessage)
                .ThenInclude(rm => rm.ConversationUser)
                    .ThenInclude(cu => cu.User)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Message>> GetMessagesForConversationAsync(Guid conversationId, int count = 50, int skip = 0)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.SentAt)
            .Skip(skip)
            .Take(count)
            .Include(m => m.ConversationUser)
                .ThenInclude(cu => cu.User)
            .Include(m => m.ReplyToMessage)
                .ThenInclude(rm => rm.ConversationUser)
                    .ThenInclude(cu => cu.User)
            .ToListAsync();
    }

    public async Task<List<Message>> GetMessagesForUserInConversationAsync(Guid conversationId, string userId, int count = 50, int skip = 0)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId && m.ConversationUser.UserId == userId)
            .OrderByDescending(m => m.SentAt)
            .Skip(skip)
            .Take(count)
            .Include(m => m.ConversationUser)
                .ThenInclude(cu => cu.User)
            .Include(m => m.ReplyToMessage)
                .ThenInclude(rm => rm.ConversationUser)
                    .ThenInclude(cu => cu.User)
            .ToListAsync();
    }

    public async Task<Message> AddAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<Message> UpdateAsync(Message message)
    {
        var existingMessage = await _context.Messages.FindAsync(message.Id);
        if (existingMessage == null)
        {
            throw new KeyNotFoundException("Message not found");
        }

        // Update the existing message properties
        _context.Entry(existingMessage).CurrentValues.SetValues(message);
        await _context.SaveChangesAsync();
        return existingMessage;
    }

    public async Task DeleteAsync(Guid id)
    {
        var message = await _context.Messages.FindAsync(id);
        if (message != null)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SoftDeleteAsync(Guid messageId, string userId)
    {
        var message = await _context.Messages
            .Include(m => m.ConversationUser)
            .FirstOrDefaultAsync(m => m.Id == messageId);
        
        if (message != null && message.ConversationUser.UserId == userId)
        {
            message.IsDeleted = true;
            message.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Message> StarMessageAsync(Guid messageId, string userId)
    {
        var message = await _context.Messages
            .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);
        
        if (message != null && !message.StarredBy.Contains(userId))
        {
            message.StarredBy.Add(userId);
            _context.Update(message);
            await _context.SaveChangesAsync();
        }
        
        return message;
    }

    public async Task<Message> UnstarMessageAsync(Guid messageId, string userId)
    {
        var message = await _context.Messages
            .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);
        
        if (message != null && message.StarredBy.Contains(userId))
        {
            message.StarredBy.Remove(userId);
            _context.Update(message);
            await _context.SaveChangesAsync();
        }
        
        return message;
    }

    public async Task<List<Message>> GetStarredMessagesAsync(Guid conversationId, string userId, int count = 50, int skip = 0)
    {
       var ret =  await _context.Messages
            .Where(m => m.ConversationId == conversationId &&
                        !m.IsDeleted)
            .OrderByDescending(m => m.SentAt)
            .Skip(skip)
            .Take(count)
            .Include(m => m.ConversationUser)
                .ThenInclude(cu => cu.User)
            .Include(m => m.ReplyToMessage)
                .ThenInclude(rm => rm.ConversationUser)
                    .ThenInclude(cu => cu.User)
            .ToListAsync();

        return ret.Where(m => m.StarredBy.Contains(userId)).ToList();
    }
}