using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IAihistoryRepository
{
    Task<IEnumerable<Aihistory>> GetAllAsync();
    Task<Aihistory?> GetByIdAsync(int historyId);
    Task<IEnumerable<Aihistory>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Aihistory>> GetByPromptInstanceIdAsync(int promptInstanceId);
    Task<Aihistory> CreateAsync(Aihistory Aihistory);
    Task<Aihistory> UpdateAsync(Aihistory Aihistory);
    Task<bool> DeleteAsync(int historyId);
    Task<bool> ExistsAsync(int historyId);
    Task<IEnumerable<Aihistory>> GetRecentHistoriesAsync(int userId, int count = 10);
    Task<int> GetHistoryCountByUserAsync(int userId);
}
