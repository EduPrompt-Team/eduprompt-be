using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IOutputDetailRepository
{
    Task<OutputDetail?> GetByIdAsync(int detailId);
    Task<IEnumerable<OutputDetail>> GetByOutputIdAsync(int outputId);
    Task<OutputDetail> CreateAsync(OutputDetail detail);
    Task<OutputDetail> UpdateAsync(OutputDetail detail);
    Task<bool> DeleteAsync(int detailId);
}


