using Eduprompt.Domain.DTOs.AIHistory;

namespace Eduprompt.Domain.Interface.Service;

public interface IAIHistoryService
{
    Task<AIHistoryDto?> GetByIdAsync(int historyId);
    Task<IEnumerable<AIHistoryDto>> GetByUserIdAsync(int userId);
    Task<IEnumerable<AIHistoryDto>> GetByPromptInstanceIdAsync(int promptInstanceId);
    Task<AIHistoryDto> CreateAsync(CreateAIHistoryDto createDto);
    Task<AIHistoryDto> UpdateAsync(int historyId, CreateAIHistoryDto updateDto);
    Task<bool> DeleteAsync(int historyId);
    Task<IEnumerable<AIHistoryDto>> GetRecentHistoriesAsync(int userId, int count = 10);
    Task<int> GetHistoryCountByUserAsync(int userId);
    Task<decimal> GetTotalCostByUserAsync(int userId);
}
