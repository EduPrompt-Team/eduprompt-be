using Eduprompt.Domain.DTOs.TemplateArchitecture;

namespace Eduprompt.Domain.Interface.Service;

public interface ITemplateArchitectureService
{
    Task<TemplateArchitectureDto?> GetByIdAsync(int architectureId);
    Task<IEnumerable<TemplateArchitectureDto>> GetAllAsync();
    Task<IEnumerable<TemplateArchitectureDto>> GetByPromptInstanceIdAsync(int promptInstanceId);
    Task<TemplateArchitectureDto> CreateAsync(CreateTemplateArchitectureDto createDto);
    Task<TemplateArchitectureDto> UpdateAsync(int architectureId, CreateTemplateArchitectureDto updateDto);
    Task<bool> DeleteAsync(int architectureId);
}


