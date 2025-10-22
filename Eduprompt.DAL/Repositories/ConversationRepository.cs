using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly EdupromptV2Context _context;

    public ConversationRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<Conversation?> GetByIdAsync(int ConversationId)
    {
        return await _context.Conversations
            .Include(c => c.User)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.ConversationId == ConversationId);
    }

    public async Task<IEnumerable<Conversation>> GetByUserIdAsync(int UserId)
    {
        return await _context.Conversations
            .Include(c => c.User)
            .Include(c => c.Messages)
            .Where(c => c.UserId == UserId)
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
            .FirstOrDefaultAsync(c => c.ConversationId == conversation.ConversationId) ?? conversation;
    }

    public async Task<Conversation> UpdateAsync(Conversation conversation)
    {
        _context.Conversations.Update(conversation);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.Conversations
            .Include(c => c.User)
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.ConversationId == conversation.ConversationId) ?? conversation;
    }

    public async Task<bool> DeleteAsync(int ConversationId)
    {
        var conversation = await _context.Conversations.FindAsync(ConversationId);
        if (conversation == null) return false;

        _context.Conversations.Remove(conversation);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int ConversationId)
    {
        return await _context.Conversations.AnyAsync(c => c.ConversationId == ConversationId);
    }

    public async Task<IEnumerable<Conversation>> GetRecentConversationsAsync(int UserId, int count = 10)
    {
        return await _context.Conversations
            .Include(c => c.User)
            .Include(c => c.Messages)
            .Where(c => c.UserId == UserId)
            .OrderByDescending(c => c.StartedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetMessageCountAsync(int ConversationId)
    {
        return await _context.Messages.CountAsync(m => m.ConversationId == ConversationId);
    }
}
