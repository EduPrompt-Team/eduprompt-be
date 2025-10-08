using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IExpectedOutputRepository
{
    Task<ExpectedOutput?> GetByIdAsync(int outputId);
    Task<IEnumerable<ExpectedOutput>> GetByInstanceIdAsync(int instanceId);
    Task<ExpectedOutput> CreateAsync(ExpectedOutput output);
    Task<ExpectedOutput> UpdateAsync(ExpectedOutput output);
    Task<bool> DeleteAsync(int outputId);
}


