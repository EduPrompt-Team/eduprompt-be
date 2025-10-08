using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IPromptInstanceDetailRepository
{
    Task<PromptInstanceDetail?> GetByIdAsync(int detailId);
    Task<IEnumerable<PromptInstanceDetail>> GetByInstanceIdAsync(int instanceId);
    Task<PromptInstanceDetail> CreateAsync(PromptInstanceDetail detail);
    Task<PromptInstanceDetail> UpdateAsync(PromptInstanceDetail detail);
    Task<bool> DeleteAsync(int detailId);
    Task<bool> ExistsAsync(int detailId);
    Task<IEnumerable<PromptInstanceDetail>> GetOrderedByInstanceIdAsync(int instanceId);
    Task<bool> DeleteByInstanceIdAsync(int instanceId);
}
