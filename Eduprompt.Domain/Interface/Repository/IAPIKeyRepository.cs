using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IApikeyRepository
{
    Task<Apikey?> GetByIdAsync(int ApikeyId);
    Task<IEnumerable<Apikey>> GetByPackageIdAsync(int packageId);
    Task<Apikey> CreateAsync(Apikey Apikey);
    Task<Apikey> UpdateAsync(Apikey Apikey);
    Task<bool> DeleteAsync(int ApikeyId);
    Task<bool> ExistsAsync(int ApikeyId);
    Task<IEnumerable<Apikey>> GetActiveKeysByPackageIdAsync(int packageId);
    Task<Apikey?> GetActiveKeyByProviderAsync(string provider);
}
