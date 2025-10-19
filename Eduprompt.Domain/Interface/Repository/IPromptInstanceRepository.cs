using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IPromptInstanceRepository
{
    Task<PromptInstance?> GetByIdAsync(int instanceId);
    Task<IEnumerable<PromptInstance>> GetAllAsync();
    Task<IEnumerable<PromptInstance>> GetByUserIdAsync(int userId);
    Task<IEnumerable<PromptInstance>> GetByTemplateIdAsync(int templateId);
    Task<PromptInstance> CreateAsync(PromptInstance promptInstance);
    Task<PromptInstance> UpdateAsync(PromptInstance promptInstance);
    Task<bool> DeleteAsync(int instanceId);
    Task<bool> ExistsAsync(int instanceId);
}
