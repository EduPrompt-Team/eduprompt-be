using Eduprompt.Domain.DTOs.PromptInstance;

namespace Eduprompt.Domain.Interface.Service;

public interface IPromptInstanceService
{
    Task<PromptInstanceDto?> GetByIdAsync(int instanceId);
    Task<IEnumerable<PromptInstanceDto>> GetByUserIdAsync(int userId);
    Task<IEnumerable<PromptInstanceDto>> GetByTemplateIdAsync(int templateId);
    Task<PromptInstanceDto> CreateAsync(CreatePromptInstanceDto createPromptInstanceDto);
    Task<PromptInstanceDto> UpdateAsync(int instanceId, UpdatePromptInstanceDto updatePromptInstanceDto);
    Task<bool> DeleteAsync(int instanceId);
    Task<IEnumerable<PromptInstanceDto>> GetByStatusAsync(string status);
    Task<IEnumerable<PromptInstanceDto>> GetRecentInstancesAsync(int userId, int count = 10);
    Task<bool> CompleteInstanceAsync(int instanceId, string outputData);
}
