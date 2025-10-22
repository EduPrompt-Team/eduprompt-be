using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class AIHistoryRepository : IAihistoryRepository
{
    private readonly EdupromptV2Context _context;

    public AIHistoryRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Aihistory>> GetAllAsync()
    {
        return await _context.Aihistories
            .Include(h => h.User)
            .Include(h => h.PromptInstance)
            .Include(h => h.Conversation)
            .ToListAsync();
    }

    public async Task<Aihistory?> GetByIdAsync(int historyId)
    {
        return await _context.Aihistories
            .Include(h => h.User)
            .Include(h => h.PromptInstance)
            .Include(h => h.Conversation)
            .FirstOrDefaultAsync(h => h.AihistoryId == historyId);
    }

    public async Task<IEnumerable<Aihistory>> GetByUserIdAsync(int UserId)
    {
        return await _context.Aihistories
            .Include(h => h.User)
            .Include(h => h.PromptInstance)
            .Include(h => h.Conversation)
            .Where(h => h.UserId == UserId)
            .OrderByDescending(h => h.ExecutedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Aihistory>> GetByPromptInstanceIdAsync(int PromptInstanceId)
    {
        return await _context.Aihistories
            .Include(h => h.User)
            .Include(h => h.PromptInstance)
            .Include(h => h.Conversation)
            .Where(h => h.PromptInstanceId == PromptInstanceId)
            .OrderByDescending(h => h.ExecutedAt)
            .ToListAsync();
    }

    public async Task<Aihistory> CreateAsync(Aihistory aiHistory)
    {
        _context.Aihistories.Add(aiHistory);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.Aihistories
            .Include(h => h.User)
            .Include(h => h.PromptInstance)
            .Include(h => h.Conversation)
            .FirstOrDefaultAsync(h => h.AihistoryId == aiHistory.AihistoryId) ?? aiHistory;
    }

    public async Task<Aihistory> UpdateAsync(Aihistory aiHistory)
    {
        _context.Aihistories.Update(aiHistory);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.Aihistories
            .Include(h => h.User)
            .Include(h => h.PromptInstance)
            .Include(h => h.Conversation)
            .FirstOrDefaultAsync(h => h.AihistoryId == aiHistory.AihistoryId) ?? aiHistory;
    }

    public async Task<bool> DeleteAsync(int historyId)
    {
        var history = await _context.Aihistories.FindAsync(historyId);
        if (history == null) return false;

        _context.Aihistories.Remove(history);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int historyId)
    {
        return await _context.Aihistories.AnyAsync(h => h.AihistoryId == historyId);
    }

    public async Task<IEnumerable<Aihistory>> GetRecentHistoriesAsync(int UserId, int count = 10)
    {
        return await _context.Aihistories
            .Include(h => h.User)
            .Include(h => h.PromptInstance)
            .Include(h => h.Conversation)
            .Where(h => h.UserId == UserId)
            .OrderByDescending(h => h.ExecutedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetHistoryCountByUserAsync(int UserId)
    {
        return await _context.Aihistories.CountAsync(h => h.UserId == UserId);
    }
}