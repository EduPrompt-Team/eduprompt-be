using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IAPIKeyRepository
{
    Task<APIKey?> GetByIdAsync(int apiKeyId);
    Task<IEnumerable<APIKey>> GetByPackageIdAsync(int packageId);
    Task<APIKey> CreateAsync(APIKey apiKey);
    Task<APIKey> UpdateAsync(APIKey apiKey);
    Task<bool> DeleteAsync(int apiKeyId);
    Task<bool> ExistsAsync(int apiKeyId);
    Task<IEnumerable<APIKey>> GetActiveKeysByPackageIdAsync(int packageId);
    Task<APIKey?> GetActiveKeyByProviderAsync(string provider);
}
