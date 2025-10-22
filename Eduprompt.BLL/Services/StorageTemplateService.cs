using AutoMapper;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class StorageTemplateService : IStorageTemplateService
{
    private readonly IStorageTemplateRepository _storageRepository;
    private readonly IPackageRepository _packageRepository;
    private readonly IMapper _mapper;

    public StorageTemplateService(
        IStorageTemplateRepository storageRepository,
        IPackageRepository packageRepository,
        IMapper mapper)
    {
        _storageRepository = storageRepository;
        _packageRepository = packageRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StorageTemplateServiceDto>> GetUserStorageAsync(int UserId)
    {
        var storage = await _storageRepository.GetByUserIdAsync(UserId);
        return storage.Select(MapToDto);
    }

    public async Task<StorageTemplateServiceDto> AddToStorageAsync(int UserId, StorageTemplateCreateServiceDto storageDto)
    {
        // Validate package exists
        var package = await _packageRepository.GetByIdAsync(storageDto.TemplateId);
        if (package == null)
        {
            throw new InvalidOperationException($"Package with ID {storageDto.TemplateId} not found");
        }

        // Check if already exists
        if (await _storageRepository.ExistsAsync(UserId, storageDto.TemplateId))
        {
            throw new InvalidOperationException("Template already in storage");
        }

        var storage = new StorageTemplate
        {
            UserId = UserId,
            PackageId = storageDto.TemplateId,
            TemplateName = package.PackageName ?? "",
            IsFavorite = false,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _storageRepository.CreateAsync(storage);
        return MapToDto(created);
    }

    public async Task<bool> RemoveFromStorageAsync(int id, int UserId)
    {
        var storage = await _storageRepository.GetByIdAsync(id);
        if (storage == null || storage.UserId != UserId)
            return false;

        return await _storageRepository.DeleteAsync(id);
    }

    public async Task<bool> IsInStorageAsync(int UserId, int templateId)
    {
        return await _storageRepository.ExistsAsync(UserId, templateId);
    }

    private static StorageTemplateServiceDto MapToDto(StorageTemplate s)
    {
        return new StorageTemplateServiceDto
        {
            StorageId = s.StorageId,
            UserId = s.UserId,
            TemplateId = s.PackageId,
            UploadDate = s.CreatedAt,
            UpdatedDate = null,
            Status = null,
            UserName = s.User?.FullName,
            TemplateName = s.Package?.PackageName,
            TemplateDescription = s.Package?.Description,
            TemplatePrice = s.Package?.Price,
            TemplatePreviewUrl = null
        };
    }
} 







