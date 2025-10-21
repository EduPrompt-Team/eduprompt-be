using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly EdupromptContext _context;

    public MessageRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<Message?> GetByIdAsync(int messageId)
    {
        return await _context.Messages
            .Include(m => m.Conversation)
            .FirstOrDefaultAsync(m => m.MessageID == messageId);
    }

    public async Task<IEnumerable<Message>> GetByConversationIdAsync(int conversationId)
    {
        return await _context.Messages
            .Include(m => m.Conversation)
            .Where(m => m.ConversationID == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task<Message> CreateAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.Messages
            .Include(m => m.Conversation)
            .FirstOrDefaultAsync(m => m.MessageID == message.MessageID) ?? message;
    }

    public async Task<Message> UpdateAsync(Message message)
    {
        _context.Messages.Update(message);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.Messages
            .Include(m => m.Conversation)
            .FirstOrDefaultAsync(m => m.MessageID == message.MessageID) ?? message;
    }

    public async Task<bool> DeleteAsync(int messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message == null) return false;

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int messageId)
    {
        return await _context.Messages.AnyAsync(m => m.MessageID == messageId);
    }

    public async Task<IEnumerable<Message>> GetRecentMessagesAsync(int conversationId, int count = 50)
    {
        return await _context.Messages
            .Include(m => m.Conversation)
            .Where(m => m.ConversationID == conversationId)
            .OrderByDescending(m => m.SentAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Message?> GetLastMessageAsync(int conversationId)
    {
        return await _context.Messages
            .Include(m => m.Conversation)
            .Where(m => m.ConversationID == conversationId)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefaultAsync();
    }
}
