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

        public async Task<IEnumerable<StorageTemplateServiceDto>> GetPublicAsync(int? packageId, string? grade, string? subject, string? chapter)
        {
            var list = await _storageRepository.GetPublicAsync(packageId, grade, subject, chapter);
            return list.Select(MapToDto);
        }

        public async Task<StorageTemplateServiceDto?> UpdateAsync(int id, int currentUserId, StorageTemplateUpdateServiceDto updateDto, bool currentUserIsAdmin)
        {
            var entity = await _storageRepository.GetByIdAsync(id);
            if (entity == null) return null;
            if (!currentUserIsAdmin && entity.UserId != currentUserId) return null;

            if (!string.IsNullOrWhiteSpace(updateDto.TemplateName)) entity.TemplateName = updateDto.TemplateName;
            if (updateDto.TemplateContent != null) entity.TemplateContent = updateDto.TemplateContent;
            if (updateDto.Grade != null) entity.Grade = updateDto.Grade;
            if (updateDto.Subject != null) entity.Subject = updateDto.Subject;
            if (updateDto.Chapter != null) entity.Chapter = updateDto.Chapter;
            if (updateDto.IsPublic.HasValue)
            {
                if (!currentUserIsAdmin && entity.UserId != currentUserId) return null;
                if (updateDto.IsPublic.Value && string.IsNullOrWhiteSpace(entity.TemplateContent))
                    throw new InvalidOperationException("TemplateContent is required to publish");
                entity.IsPublic = updateDto.IsPublic.Value;
            }

            var saved = await _storageRepository.UpdateAsync(entity);
            return saved == null ? null : MapToDto(saved);
        }

        public async Task<bool> PublishAsync(int id, bool isPublish, int currentUserId, bool currentUserIsAdmin)
        {
            var entity = await _storageRepository.GetByIdAsync(id);
            if (entity == null) return false;
            if (!currentUserIsAdmin && entity.UserId != currentUserId) return false;
            if (isPublish && string.IsNullOrWhiteSpace(entity.TemplateContent)) return false;
            return await _storageRepository.SetPublishAsync(id, isPublish);
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
            Status = s.IsPublic ? "Public" : "Private",
            UserName = s.User?.FullName,
            TemplateName = s.Package?.PackageName,
            TemplateDescription = s.Package?.Description,
            TemplatePrice = s.Package?.Price,
            TemplatePreviewUrl = null
        };
    }
} 







