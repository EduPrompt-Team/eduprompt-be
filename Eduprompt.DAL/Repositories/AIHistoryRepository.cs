using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class AIHistoryRepository : IAIHistoryRepository
{
    private readonly EdupromptContext _context;

    public AIHistoryRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<AIHistory?> GetByIdAsync(int historyId)
    {
        return await _context.AIHistories
            .Include(h => h.User)
            .Include(h => h.PromptInstance)
            .FirstOrDefaultAsync(h => h.AIHistoryID == historyId);
    }

    public async Task<IEnumerable<AIHistory>> GetByUserIdAsync(int userId)
    {
        return await _context.AIHistories
            .Include(h => h.User)
            .Include(h => h.PromptInstance)
            .Where(h => h.UserID == userId)
            .OrderByDescending(h => h.CreatedDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<AIHistory>> GetByPromptInstanceIdAsync(int promptInstanceId)
    {
        return await _context.AIHistories
            .Include(h => h.User)
            .Include(h => h.PromptInstance)
            .Where(h => h.PromptInstanceID == promptInstanceId)
            .OrderByDescending(h => h.CreatedDate)
            .ToListAsync();
    }

    public async Task<AIHistory> CreateAsync(AIHistory aiHistory)
    {
        _context.AIHistories.Add(aiHistory);
        await _context.SaveChangesAsync();
        return aiHistory;
    }

    public async Task<AIHistory> UpdateAsync(AIHistory aiHistory)
    {
        _context.AIHistories.Update(aiHistory);
        await _context.SaveChangesAsync();
        return aiHistory;
    }

    public async Task<bool> DeleteAsync(int historyId)
    {
        var history = await _context.AIHistories.FindAsync(historyId);
        if (history == null) return false;

        _context.AIHistories.Remove(history);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int historyId)
    {
        return await _context.AIHistories.AnyAsync(h => h.AIHistoryID == historyId);
    }

    public async Task<IEnumerable<AIHistory>> GetRecentHistoriesAsync(int userId, int count = 10)
    {
        return await _context.AIHistories
            .Include(h => h.User)
            .Include(h => h.PromptInstance)
            .Where(h => h.UserID == userId)
            .OrderByDescending(h => h.CreatedDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetHistoryCountByUserAsync(int userId)
    {
        return await _context.AIHistories.CountAsync(h => h.UserID == userId);
    }

    public async Task<decimal> GetTotalCostByUserAsync(int userId)
    {
        return await _context.AIHistories
            .Where(h => h.UserID == userId && h.Cost.HasValue)
            .SumAsync(h => h.Cost ?? 0);
    }
}
