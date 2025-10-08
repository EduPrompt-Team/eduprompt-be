using Eduprompt.Domain.DTOs.APIKey;

namespace Eduprompt.Domain.Interface.Service;

public interface IAPIKeyService
{
    Task<APIKeyDto?> GetByIdAsync(int apiKeyId);
    Task<IEnumerable<APIKeyDto>> GetByPackageIdAsync(int packageId);
    Task<IEnumerable<APIKeyDto>> GetActiveKeysByPackageIdAsync(int packageId);
    Task<APIKeyDto?> GetActiveKeyByProviderAsync(string provider);
    Task<APIKeyDto> CreateAsync(CreateAPIKeyDto createDto);
    Task<APIKeyDto> UpdateAsync(int apiKeyId, CreateAPIKeyDto updateDto);
    Task<bool> DeleteAsync(int apiKeyId);
}


