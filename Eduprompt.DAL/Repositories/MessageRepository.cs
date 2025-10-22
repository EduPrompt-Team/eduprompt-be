using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly EdupromptV2Context _context;

    public MessageRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<Message?> GetByIdAsync(int MessageId)
    {
        return await _context.Messages
            .Include(m => m.Conversation)
            .FirstOrDefaultAsync(m => m.MessageId == MessageId);
    }

    public async Task<IEnumerable<Message>> GetByConversationIdAsync(int ConversationId)
    {
        return await _context.Messages
            .Include(m => m.Conversation)
            .Where(m => m.ConversationId == ConversationId)
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
            .FirstOrDefaultAsync(m => m.MessageId == message.MessageId) ?? message;
    }

    public async Task<Message> UpdateAsync(Message message)
    {
        _context.Messages.Update(message);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.Messages
            .Include(m => m.Conversation)
            .FirstOrDefaultAsync(m => m.MessageId == message.MessageId) ?? message;
    }

    public async Task<bool> DeleteAsync(int MessageId)
    {
        var message = await _context.Messages.FindAsync(MessageId);
        if (message == null) return false;

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int MessageId)
    {
        return await _context.Messages.AnyAsync(m => m.MessageId == MessageId);
    }

    public async Task<IEnumerable<Message>> GetRecentMessagesAsync(int ConversationId, int count = 50)
    {
        return await _context.Messages
            .Include(m => m.Conversation)
            .Where(m => m.ConversationId == ConversationId)
            .OrderByDescending(m => m.SentAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Message?> GetLastMessageAsync(int ConversationId)
    {
        return await _context.Messages
            .Include(m => m.Conversation)
            .Where(m => m.ConversationId == ConversationId)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefaultAsync();
    }
}
