using Eduprompt.Domain.DTOs.APIKey;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class APIKeyService : IAPIKeyService
{
    private readonly IAPIKeyRepository _apiKeyRepository;
    private readonly IPackageRepository _packageRepository;

    public APIKeyService(IAPIKeyRepository apiKeyRepository, IPackageRepository packageRepository)
    {
        _apiKeyRepository = apiKeyRepository;
        _packageRepository = packageRepository;
    }

    public async Task<APIKeyDto?> GetByIdAsync(int apiKeyId)
    {
        var entity = await _apiKeyRepository.GetByIdAsync(apiKeyId);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<IEnumerable<APIKeyDto>> GetByPackageIdAsync(int packageId)
    {
        var items = await _apiKeyRepository.GetByPackageIdAsync(packageId);
        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<APIKeyDto>> GetActiveKeysByPackageIdAsync(int packageId)
    {
        var items = await _apiKeyRepository.GetActiveKeysByPackageIdAsync(packageId);
        return items.Select(MapToDto);
    }

    public async Task<APIKeyDto?> GetActiveKeyByProviderAsync(string provider)
    {
        var item = await _apiKeyRepository.GetActiveKeyByProviderAsync(provider);
        return item == null ? null : MapToDto(item);
    }

    public async Task<APIKeyDto> CreateAsync(CreateAPIKeyDto createDto)
    {
        // Ensure package exists
        var package = await _packageRepository.GetByIdAsync(createDto.PackageID);
        if (package == null) throw new ArgumentException("Package not found");

        var entity = new APIKey
        {
            PackageID = createDto.PackageID,
            APIProvider = createDto.APIProvider,
            KeyHash = createDto.KeyHash,
            UsageLimit = createDto.UsageLimit,
            CurrentUsage = 0,
            ExpiresAt = createDto.ExpiresAt
        };

        var created = await _apiKeyRepository.CreateAsync(entity);
        return MapToDto(created);
    }

    public async Task<APIKeyDto> UpdateAsync(int apiKeyId, CreateAPIKeyDto updateDto)
    {
        var apiKey = await _apiKeyRepository.GetByIdAsync(apiKeyId);
        if (apiKey == null) throw new KeyNotFoundException("API key not found");

        apiKey.APIProvider = updateDto.APIProvider;
        apiKey.KeyHash = updateDto.KeyHash;
        apiKey.UsageLimit = updateDto.UsageLimit;
        apiKey.ExpiresAt = updateDto.ExpiresAt;

        var updatedApiKey = await _apiKeyRepository.UpdateAsync(apiKey);
        return MapToDto(updatedApiKey);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _apiKeyRepository.DeleteAsync(id);
    }

    private static APIKeyDto MapToDto(APIKey entity)
    {
        return new APIKeyDto
        {
            APIKeyID = entity.APIKeyID,
            PackageID = entity.PackageID,
            APIProvider = entity.APIProvider,
            KeyHash = entity.KeyHash,
            UsageLimit = entity.UsageLimit,
            CurrentUsage = entity.CurrentUsage,
            ExpiresAt = entity.ExpiresAt,
            PackageName = entity.Package?.PackageName
        };
    }
}