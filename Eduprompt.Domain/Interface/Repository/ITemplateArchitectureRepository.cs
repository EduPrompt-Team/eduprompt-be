using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface ITemplateArchitectureRepository
{
    Task<TemplateArchitecture?> GetByIdAsync(int architectureId);
    Task<IEnumerable<TemplateArchitecture>> GetByInstanceIdAsync(int instanceId);
    Task<TemplateArchitecture> CreateAsync(TemplateArchitecture architecture);
    Task<TemplateArchitecture> UpdateAsync(TemplateArchitecture architecture);
    Task<bool> DeleteAsync(int architectureId);
    Task<bool> ExistsAsync(int architectureId);
    Task<IEnumerable<TemplateArchitecture>> GetByStorageIdAsync(int storageId);
}
