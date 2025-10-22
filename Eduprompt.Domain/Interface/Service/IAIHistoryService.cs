using Eduprompt.Domain.DTOs.Aihistory;

namespace Eduprompt.Domain.Interface.Service;

public interface IAihistoryService
{
    Task<IEnumerable<AihistoryDto>> GetAllAsync();
    Task<AihistoryDto?> GetByIdAsync(int historyId);
    Task<IEnumerable<AihistoryDto>> GetByUserIdAsync(int userId);
    Task<IEnumerable<AihistoryDto>> GetByPromptInstanceIdAsync(int promptInstanceId);
    Task<AihistoryDto> CreateAsync(CreateAihistoryDto createDto);
    Task<AihistoryDto> UpdateAsync(int historyId, CreateAihistoryDto updateDto);
    Task<bool> DeleteAsync(int historyId);
    Task<IEnumerable<AihistoryDto>> GetRecentHistoriesAsync(int userId, int count = 10);
    Task<int> GetHistoryCountByUserAsync(int userId);
    Task<decimal> GetTotalCostByUserAsync(int userId);
}
