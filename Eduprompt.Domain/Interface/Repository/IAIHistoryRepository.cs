using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IAIHistoryRepository
{
    Task<AIHistory?> GetByIdAsync(int historyId);
    Task<IEnumerable<AIHistory>> GetByUserIdAsync(int userId);
    Task<IEnumerable<AIHistory>> GetByPromptInstanceIdAsync(int promptInstanceId);
    Task<AIHistory> CreateAsync(AIHistory aiHistory);
    Task<AIHistory> UpdateAsync(AIHistory aiHistory);
    Task<bool> DeleteAsync(int historyId);
    Task<bool> ExistsAsync(int historyId);
    Task<IEnumerable<AIHistory>> GetRecentHistoriesAsync(int userId, int count = 10);
    Task<int> GetHistoryCountByUserAsync(int userId);
    Task<decimal> GetTotalCostByUserAsync(int userId);
}
