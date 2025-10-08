using Eduprompt.Domain.DTOs.ExpectedOutput;

namespace Eduprompt.Domain.Interface.Service;

public interface IExpectedOutputService
{
    Task<ExpectedOutputDto?> GetByIdAsync(int outputId);
    Task<IEnumerable<ExpectedOutputDto>> GetByInstanceIdAsync(int instanceId);
    Task<ExpectedOutputDto> CreateAsync(CreateExpectedOutputDto createDto);
    Task<ExpectedOutputDto> UpdateAsync(int outputId, CreateExpectedOutputDto updateDto);
    Task<bool> DeleteAsync(int outputId);
}


