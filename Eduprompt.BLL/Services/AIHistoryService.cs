using Eduprompt.Domain.DTOs.Aihistory;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class AihistoryService : IAihistoryService
{
    private readonly IAihistoryRepository _aiHistoryRepository;

    public AihistoryService(IAihistoryRepository aiHistoryRepository)
    {
        _aiHistoryRepository = aiHistoryRepository;
    }

    public async Task<IEnumerable<AihistoryDto>> GetAllAsync()
    {
        var histories = await _aiHistoryRepository.GetAllAsync();
        return histories.Select(MapToDto);
    }

    public async Task<AihistoryDto?> GetByIdAsync(int historyId)
    {
        var history = await _aiHistoryRepository.GetByIdAsync(historyId);
        if (history == null) return null;

        return MapToDto(history);
    }

    public async Task<IEnumerable<AihistoryDto>> GetByUserIdAsync(int UserId)
    {
        var histories = await _aiHistoryRepository.GetByUserIdAsync(UserId);
        return histories.Select(MapToDto);
    }

    public async Task<IEnumerable<AihistoryDto>> GetByPromptInstanceIdAsync(int PromptInstanceId)
    {
        var histories = await _aiHistoryRepository.GetByPromptInstanceIdAsync(PromptInstanceId);
        return histories.Select(MapToDto);
    }

    public async Task<AihistoryDto> CreateAsync(CreateAihistoryDto createDto)
    {
        var history = new Aihistory
        {
            UserId = createDto.UserId,
            ConversationId = createDto.ConversationId,
            PromptInstanceId = createDto.PromptInstanceId,
            UserMessage = createDto.UserMessage,
            Airesponse = createDto.Airesponse,
            ExecutedAt = DateTime.UtcNow,
            ProcessingTimeMs = createDto.ProcessingTimeMs,
            Status = createDto.Status ?? "Completed"
        };

        var createdHistory = await _aiHistoryRepository.CreateAsync(history);
        return MapToDto(createdHistory);
    }

    public async Task<AihistoryDto> UpdateAsync(int historyId, CreateAihistoryDto updateDto)
    {
        var history = await _aiHistoryRepository.GetByIdAsync(historyId);
        if (history == null) throw new KeyNotFoundException("AI history not found");

        history.UserId = updateDto.UserId;
        history.ConversationId = updateDto.ConversationId;
        history.PromptInstanceId = updateDto.PromptInstanceId;
        history.UserMessage = updateDto.UserMessage;
        history.Airesponse = updateDto.Airesponse;
        history.ProcessingTimeMs = updateDto.ProcessingTimeMs;
        history.Status = updateDto.Status ?? history.Status;

        var updatedHistory = await _aiHistoryRepository.UpdateAsync(history);
        return MapToDto(updatedHistory);
    }

    public async Task<bool> DeleteAsync(int historyId)
    {
        return await _aiHistoryRepository.DeleteAsync(historyId);
    }

    public async Task<IEnumerable<AihistoryDto>> GetRecentHistoriesAsync(int UserId, int count = 10)
    {
        var histories = await _aiHistoryRepository.GetRecentHistoriesAsync(UserId, count);
        return histories.Select(MapToDto);
    }

    public async Task<int> GetHistoryCountByUserAsync(int UserId)
    {
        return await _aiHistoryRepository.GetHistoryCountByUserAsync(UserId);
    }

    public async Task<decimal> GetTotalCostByUserAsync(int UserId)
    {
        var histories = await _aiHistoryRepository.GetByUserIdAsync(UserId);
        return histories.Sum(h => h.ProcessingTimeMs ?? 0);
    }

    private static AihistoryDto MapToDto(Aihistory history)
    {
        return new AihistoryDto
        {
            HistoryID = history.AihistoryId,
            UserId = history.UserId,
            ConversationId = history.ConversationId,
            PromptInstanceId = history.PromptInstanceId,
            UserMessage = history.UserMessage,
            Airesponse = history.Airesponse,
            ExecutedAt = history.ExecutedAt,
            ProcessingTimeMs = history.ProcessingTimeMs,
            Status = history.Status,
            UserName = history.User?.FullName,
            PromptInstanceName = history.PromptInstance?.PromptName
        };
    }
}