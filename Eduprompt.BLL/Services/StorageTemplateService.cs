using AutoMapper;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class StorageTemplateService : IStorageTemplateService
{
    private readonly IStorageTemplateRepository _storageRepository;
    private readonly IMapper _mapper;

    public StorageTemplateService(
        IStorageTemplateRepository storageRepository,
        IMapper mapper)
    {
        _storageRepository = storageRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StorageTemplateServiceDto>> GetUserStorageAsync(int userId)
    {
        var storage = await _storageRepository.GetByUserIdAsync(userId);
        return storage.Select(MapToDto);
    }

    public async Task<StorageTemplateServiceDto> AddToStorageAsync(int userId, StorageTemplateCreateServiceDto storageDto)
    {
        // Check if already exists
        if (await _storageRepository.ExistsAsync(userId, storageDto.TemplateId))
        {
            throw new InvalidOperationException("Template already in storage");
        }

        var storage = new StorageTemplate
        {
            UserID = userId,
            PackageID = storageDto.TemplateId,
            TemplateName = "",
            IsFavorite = false,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _storageRepository.CreateAsync(storage);
        return MapToDto(created);
    }

    public async Task<bool> RemoveFromStorageAsync(int id, int userId)
    {
        var storage = await _storageRepository.GetByIdAsync(id);
        if (storage == null || storage.UserID != userId)
            return false;

        return await _storageRepository.DeleteAsync(id);
    }

    public async Task<bool> IsInStorageAsync(int userId, int templateId)
    {
        return await _storageRepository.ExistsAsync(userId, templateId);
    }

    private static StorageTemplateServiceDto MapToDto(StorageTemplate s)
    {
        return new StorageTemplateServiceDto
        {
            StorageId = s.StorageID,
            UserId = s.UserID,
            TemplateId = s.PackageID,
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







