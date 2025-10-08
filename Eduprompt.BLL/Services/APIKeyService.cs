using Eduprompt.Domain.DTOs.APIKey;
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
        return entity == null ? null : Map(entity);
    }

    public async Task<IEnumerable<APIKeyDto>> GetByPackageIdAsync(int packageId)
    {
        var items = await _apiKeyRepository.GetByPackageIdAsync(packageId);
        return items.Select(Map);
    }

    public async Task<IEnumerable<APIKeyDto>> GetActiveKeysByPackageIdAsync(int packageId)
    {
        var items = await _apiKeyRepository.GetActiveKeysByPackageIdAsync(packageId);
        return items.Select(Map);
    }

    public async Task<APIKeyDto?> GetActiveKeyByProviderAsync(string provider)
    {
        var item = await _apiKeyRepository.GetActiveKeyByProviderAsync(provider);
        return item == null ? null : Map(item);
    }

    public async Task<APIKeyDto> CreateAsync(CreateAPIKeyDto createDto)
    {
        // ensure package exists
        var pkg = await _packageRepository.GetByIdAsync(createDto.PackageID);
        if (pkg == null) throw new ArgumentException("Package not found");

        var entity = new Eduprompt.Domain.Entities.APIKey
        {
            PackageID = createDto.PackageID,
            KeyName = createDto.KeyName,
            KeyValue = createDto.KeyValue ?? string.Empty,
            Provider = createDto.Provider ?? string.Empty,
            ExpiryDate = createDto.ExpiryDate,
            Status = createDto.Status,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        var created = await _apiKeyRepository.CreateAsync(entity);
        return Map(created);
    }

    public async Task<APIKeyDto> UpdateAsync(int apiKeyId, CreateAPIKeyDto updateDto)
    {
        var entity = await _apiKeyRepository.GetByIdAsync(apiKeyId);
        if (entity == null) throw new KeyNotFoundException("API key not found");

        if (updateDto.PackageID != 0) entity.PackageID = updateDto.PackageID;
        if (!string.IsNullOrEmpty(updateDto.KeyName)) entity.KeyName = updateDto.KeyName;
        if (updateDto.KeyValue != null) entity.KeyValue = updateDto.KeyValue;
        if (updateDto.Provider != null) entity.Provider = updateDto.Provider;
        entity.ExpiryDate = updateDto.ExpiryDate;
        if (updateDto.Status != null) entity.Status = updateDto.Status;

        var updated = await _apiKeyRepository.UpdateAsync(entity);
        return Map(updated);
    }

    public async Task<bool> DeleteAsync(int apiKeyId)
    {
        return await _apiKeyRepository.DeleteAsync(apiKeyId);
    }

    private static APIKeyDto Map(Eduprompt.Domain.Entities.APIKey e)
    {
        return new APIKeyDto
        {
            APIKeyID = e.APIKeyID,
            PackageID = e.PackageID,
            KeyName = e.KeyName,
            KeyValue = e.KeyValue,
            Provider = e.Provider,
            CreatedDate = e.CreatedDate,
            ExpiryDate = e.ExpiryDate,
            Status = e.Status,
            PackageName = e.Package?.PackageName
        };
    }
}


