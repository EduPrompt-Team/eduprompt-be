using Eduprompt.Domain.DTOs.Apikey;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class ApikeyService : IApikeyService
{
    private readonly IApikeyRepository _apiKeyRepository;
    private readonly IPackageRepository _packageRepository;

    public ApikeyService(IApikeyRepository apiKeyRepository, IPackageRepository packageRepository)
    {
        _apiKeyRepository = apiKeyRepository;
        _packageRepository = packageRepository;
    }

    public async Task<ApikeyDto?> GetByIdAsync(int ApikeyId)
    {
        var entity = await _apiKeyRepository.GetByIdAsync(ApikeyId);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<IEnumerable<ApikeyDto>> GetByPackageIdAsync(int PackageId)
    {
        var items = await _apiKeyRepository.GetByPackageIdAsync(PackageId);
        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<ApikeyDto>> GetActiveKeysByPackageIdAsync(int PackageId)
    {
        var items = await _apiKeyRepository.GetActiveKeysByPackageIdAsync(PackageId);
        return items.Select(MapToDto);
    }

    public async Task<ApikeyDto?> GetActiveKeyByProviderAsync(string provider)
    {
        var item = await _apiKeyRepository.GetActiveKeyByProviderAsync(provider);
        return item == null ? null : MapToDto(item);
    }

    public async Task<ApikeyDto> CreateAsync(CreateApikeyDto createDto)
    {
        // Ensure package exists
        var package = await _packageRepository.GetByIdAsync(createDto.PackageId);
        if (package == null) throw new ArgumentException("Package not found");

        var entity = new Apikey
        {
            PackageId = createDto.PackageId,
            Apiprovider = createDto.Apiprovider,
            KeyHash = createDto.KeyHash,
            UsageLimit = createDto.UsageLimit,
            CurrentUsage = 0,
            ExpiresAt = createDto.ExpiresAt
        };

        var created = await _apiKeyRepository.CreateAsync(entity);
        return MapToDto(created);
    }

    public async Task<ApikeyDto> UpdateAsync(int ApikeyId, CreateApikeyDto updateDto)
    {
        var apiKey = await _apiKeyRepository.GetByIdAsync(ApikeyId);
        if (apiKey == null) throw new KeyNotFoundException("API key not found");

        apiKey.Apiprovider = updateDto.Apiprovider;
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

    private static ApikeyDto MapToDto(Apikey entity)
    {
        return new ApikeyDto
        {
            ApikeyId = entity.ApikeyId,
            PackageId = entity.PackageId,
            Apiprovider = entity.Apiprovider,
            KeyHash = entity.KeyHash,
            UsageLimit = entity.UsageLimit,
            CurrentUsage = entity.CurrentUsage,
            ExpiresAt = entity.ExpiresAt,
            PackageName = entity.Package?.PackageName
        };
    }
}