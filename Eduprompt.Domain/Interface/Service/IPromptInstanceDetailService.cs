using Eduprompt.Domain.DTOs.PromptInstanceDetail;

namespace Eduprompt.Domain.Interface.Service;

public interface IPromptInstanceDetailService
{
    Task<PromptInstanceDetailDto?> GetByIdAsync(int detailId);
    Task<IEnumerable<PromptInstanceDetailDto>> GetByInstanceIdAsync(int instanceId);
    Task<PromptInstanceDetailDto> CreateAsync(CreatePromptInstanceDetailDto createDto);
    Task<PromptInstanceDetailDto> UpdateAsync(int detailId, CreatePromptInstanceDetailDto updateDto);
    Task<bool> DeleteAsync(int detailId);
}


