using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly EdupromptContext _context;

    public ConversationRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<Conversation?> GetByIdAsync(int conversationId)
    {
        return await _context.Conversations
            .Include(c => c.User)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.ConversationID == conversationId);
    }

    public async Task<IEnumerable<Conversation>> GetByUserIdAsync(int userId)
    {
        return await _context.Conversations
            .Include(c => c.User)
            .Include(c => c.Messages)
            .Where(c => c.UserID == userId)
            .OrderByDescending(c => c.StartedAt)
            .ToListAsync();
    }

    public async Task<Conversation> CreateAsync(Conversation conversation)
    {
        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.Conversations
            .Include(c => c.User)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.ConversationID == conversation.ConversationID) ?? conversation;
    }

    public async Task<Conversation> UpdateAsync(Conversation conversation)
    {
        _context.Conversations.Update(conversation);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.Conversations
            .Include(c => c.User)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.ConversationID == conversation.ConversationID) ?? conversation;
    }

    public async Task<bool> DeleteAsync(int conversationId)
    {
        var conversation = await _context.Conversations.FindAsync(conversationId);
        if (conversation == null) return false;

        _context.Conversations.Remove(conversation);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int conversationId)
    {
        return await _context.Conversations.AnyAsync(c => c.ConversationID == conversationId);
    }

    public async Task<IEnumerable<Conversation>> GetRecentConversationsAsync(int userId, int count = 10)
    {
        return await _context.Conversations
            .Include(c => c.User)
            .Include(c => c.Messages)
            .Where(c => c.UserID == userId)
            .OrderByDescending(c => c.StartedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetMessageCountAsync(int conversationId)
    {
        return await _context.Messages.CountAsync(m => m.ConversationID == conversationId);
    }
}
