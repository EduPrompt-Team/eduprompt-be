using Eduprompt.Domain.DTOs.AIHistory;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class AIHistoryService : IAIHistoryService
{
    private readonly IAIHistoryRepository _aiHistoryRepository;

    public AIHistoryService(IAIHistoryRepository aiHistoryRepository)
    {
        _aiHistoryRepository = aiHistoryRepository;
    }

    public async Task<AIHistoryDto?> GetByIdAsync(int historyId)
    {
        var history = await _aiHistoryRepository.GetByIdAsync(historyId);
        if (history == null) return null;

        return new AIHistoryDto
        {
            HistoryID = history.AIHistoryID,
            UserID = history.UserID,
            PromptInstanceID = history.PromptInstanceID,
            InputText = history.InputText,
            OutputText = history.OutputText,
            ModelUsed = history.ModelUsed,
            TokensUsed = history.TokensUsed,
            Cost = history.Cost,
            CreatedDate = history.CreatedDate,
            Status = history.Status,
            UserName = history.User?.FullName,
            InstanceName = history.PromptInstance?.InstanceName
        };
    }

    public async Task<IEnumerable<AIHistoryDto>> GetByUserIdAsync(int userId)
    {
        var histories = await _aiHistoryRepository.GetByUserIdAsync(userId);
        return histories.Select(h => new AIHistoryDto
        {
            HistoryID = h.AIHistoryID,
            UserID = h.UserID,
            PromptInstanceID = h.PromptInstanceID,
            InputText = h.InputText,
            OutputText = h.OutputText,
            ModelUsed = h.ModelUsed,
            TokensUsed = h.TokensUsed,
            Cost = h.Cost,
            CreatedDate = h.CreatedDate,
            Status = h.Status,
            UserName = h.User?.FullName,
            InstanceName = h.PromptInstance?.InstanceName
        });
    }

    public async Task<IEnumerable<AIHistoryDto>> GetByPromptInstanceIdAsync(int promptInstanceId)
    {
        var histories = await _aiHistoryRepository.GetByPromptInstanceIdAsync(promptInstanceId);
        return histories.Select(h => new AIHistoryDto
        {
            HistoryID = h.AIHistoryID,
            UserID = h.UserID,
            PromptInstanceID = h.PromptInstanceID,
            InputText = h.InputText,
            OutputText = h.OutputText,
            ModelUsed = h.ModelUsed,
            TokensUsed = h.TokensUsed,
            Cost = h.Cost,
            CreatedDate = h.CreatedDate,
            Status = h.Status,
            UserName = h.User?.FullName,
            InstanceName = h.PromptInstance?.InstanceName
        });
    }

    public async Task<AIHistoryDto> CreateAsync(CreateAIHistoryDto createDto)
    {
        var history = new Eduprompt.Domain.Entities.AIHistory
        {
            UserID = createDto.UserID,
            PromptInstanceID = createDto.PromptInstanceID,
            InputText = createDto.InputText,
            OutputText = createDto.OutputText,
            ModelUsed = createDto.ModelUsed,
            TokensUsed = createDto.TokensUsed,
            Cost = createDto.Cost,
            Status = createDto.Status ?? "Completed",
            CreatedDate = DateTime.UtcNow
        };

        var createdHistory = await _aiHistoryRepository.CreateAsync(history);
        return new AIHistoryDto
        {
            HistoryID = createdHistory.AIHistoryID,
            UserID = createdHistory.UserID,
            PromptInstanceID = createdHistory.PromptInstanceID,
            InputText = createdHistory.InputText,
            OutputText = createdHistory.OutputText,
            ModelUsed = createdHistory.ModelUsed,
            TokensUsed = createdHistory.TokensUsed,
            Cost = createdHistory.Cost,
            CreatedDate = createdHistory.CreatedDate,
            Status = createdHistory.Status,
            UserName = createdHistory.User?.FullName,
            InstanceName = createdHistory.PromptInstance?.InstanceName
        };
    }

    public async Task<AIHistoryDto> UpdateAsync(int historyId, CreateAIHistoryDto updateDto)
    {
        var history = await _aiHistoryRepository.GetByIdAsync(historyId);
        if (history == null)
            throw new KeyNotFoundException("AI History not found");

        history.InputText = updateDto.InputText;
        history.OutputText = updateDto.OutputText;
        history.ModelUsed = updateDto.ModelUsed;
        history.TokensUsed = updateDto.TokensUsed;
        history.Cost = updateDto.Cost;
        history.Status = updateDto.Status ?? history.Status;
        // history.UpdatedDate = DateTime.UtcNow; // AIHistory entity doesn't have UpdatedDate property

        var updatedHistory = await _aiHistoryRepository.UpdateAsync(history);
        return new AIHistoryDto
        {
            HistoryID = updatedHistory.AIHistoryID,
            UserID = updatedHistory.UserID,
            PromptInstanceID = updatedHistory.PromptInstanceID,
            InputText = updatedHistory.InputText,
            OutputText = updatedHistory.OutputText,
            ModelUsed = updatedHistory.ModelUsed,
            TokensUsed = updatedHistory.TokensUsed,
            Cost = updatedHistory.Cost,
            CreatedDate = updatedHistory.CreatedDate,
            Status = updatedHistory.Status,
            UserName = updatedHistory.User?.FullName,
            InstanceName = updatedHistory.PromptInstance?.InstanceName
        };
    }

    public async Task<bool> DeleteAsync(int historyId)
    {
        return await _aiHistoryRepository.DeleteAsync(historyId);
    }

    public async Task<IEnumerable<AIHistoryDto>> GetRecentHistoriesAsync(int userId, int count = 10)
    {
        var histories = await _aiHistoryRepository.GetRecentHistoriesAsync(userId, count);
        return histories.Select(h => new AIHistoryDto
        {
            HistoryID = h.AIHistoryID,
            UserID = h.UserID,
            PromptInstanceID = h.PromptInstanceID,
            InputText = h.InputText,
            OutputText = h.OutputText,
            ModelUsed = h.ModelUsed,
            TokensUsed = h.TokensUsed,
            Cost = h.Cost,
            CreatedDate = h.CreatedDate,
            Status = h.Status,
            UserName = h.User?.FullName,
            InstanceName = h.PromptInstance?.InstanceName
        });
    }

    public async Task<int> GetHistoryCountByUserAsync(int userId)
    {
        return await _aiHistoryRepository.GetHistoryCountByUserAsync(userId);
    }

    public async Task<decimal> GetTotalCostByUserAsync(int userId)
    {
        return await _aiHistoryRepository.GetTotalCostByUserAsync(userId);
    }
}
