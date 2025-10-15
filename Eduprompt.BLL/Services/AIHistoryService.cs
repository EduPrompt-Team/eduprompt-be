using Eduprompt.Domain.DTOs.AIHistory;
using Eduprompt.Domain.Entities;
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

        return MapToDto(history);
    }

    public async Task<IEnumerable<AIHistoryDto>> GetByUserIdAsync(int userId)
    {
        var histories = await _aiHistoryRepository.GetByUserIdAsync(userId);
        return histories.Select(MapToDto);
    }

    public async Task<IEnumerable<AIHistoryDto>> GetByPromptInstanceIdAsync(int promptInstanceId)
    {
        var histories = await _aiHistoryRepository.GetByPromptInstanceIdAsync(promptInstanceId);
        return histories.Select(MapToDto);
    }

    public async Task<AIHistoryDto> CreateAsync(CreateAIHistoryDto createDto)
    {
        var history = new AIHistory
        {
            UserID = createDto.UserID,
            ConversationID = createDto.ConversationID,
            PromptInstanceID = createDto.PromptInstanceID,
            UserMessage = createDto.UserMessage,
            AIResponse = createDto.AIResponse,
            ExecutedAt = DateTime.UtcNow,
            ProcessingTimeMs = createDto.ProcessingTimeMs,
            Status = createDto.Status ?? "Completed"
        };

        var createdHistory = await _aiHistoryRepository.CreateAsync(history);
        return MapToDto(createdHistory);
    }

    public async Task<AIHistoryDto> UpdateAsync(int historyId, CreateAIHistoryDto updateDto)
    {
        var history = await _aiHistoryRepository.GetByIdAsync(historyId);
        if (history == null) throw new KeyNotFoundException("AI history not found");

        history.UserID = updateDto.UserID;
        history.ConversationID = updateDto.ConversationID;
        history.PromptInstanceID = updateDto.PromptInstanceID;
        history.UserMessage = updateDto.UserMessage;
        history.AIResponse = updateDto.AIResponse;
        history.ProcessingTimeMs = updateDto.ProcessingTimeMs;
        history.Status = updateDto.Status ?? history.Status;

        var updatedHistory = await _aiHistoryRepository.UpdateAsync(history);
        return MapToDto(updatedHistory);
    }

    public async Task<bool> DeleteAsync(int historyId)
    {
        return await _aiHistoryRepository.DeleteAsync(historyId);
    }

    public async Task<IEnumerable<AIHistoryDto>> GetRecentHistoriesAsync(int userId, int count = 10)
    {
        var histories = await _aiHistoryRepository.GetRecentHistoriesAsync(userId, count);
        return histories.Select(MapToDto);
    }

    public async Task<int> GetHistoryCountByUserAsync(int userId)
    {
        return await _aiHistoryRepository.GetHistoryCountByUserAsync(userId);
    }

    public async Task<decimal> GetTotalCostByUserAsync(int userId)
    {
        var histories = await _aiHistoryRepository.GetByUserIdAsync(userId);
        return histories.Sum(h => h.ProcessingTimeMs ?? 0);
    }

    private static AIHistoryDto MapToDto(AIHistory history)
    {
        return new AIHistoryDto
        {
            HistoryID = history.AIHistoryID,
            UserID = history.UserID,
            ConversationID = history.ConversationID,
            PromptInstanceID = history.PromptInstanceID,
            UserMessage = history.UserMessage,
            AIResponse = history.AIResponse,
            ExecutedAt = history.ExecutedAt,
            ProcessingTimeMs = history.ProcessingTimeMs,
            Status = history.Status,
            UserName = history.User?.FullName,
            InstanceName = history.PromptInstance?.PromptName
        };
    }
}