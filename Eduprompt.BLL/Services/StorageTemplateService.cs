using AutoMapper;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class StorageTemplateService : IStorageTemplateService
{
    private readonly IStorageTemplateRepository _storageRepository;
        private readonly IStorageTemplateRepository _templateRepository;
    private readonly IMapper _mapper;

    public StorageTemplateService(
        IStorageTemplateRepository storageRepository,
        IStorageTemplateRepository templateRepository,
        IMapper mapper)
    {
        _storageRepository = storageRepository;
        _templateRepository = templateRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StorageTemplateServiceDto>> GetUserStorageAsync(int userId)
    {
        var storage = await _storageRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<StorageTemplateServiceDto>>(storage);
    }

    public async Task<StorageTemplateServiceDto> AddToStorageAsync(int userId, StorageTemplateCreateServiceDto storageDto)
    {
        // Validate template exists
        if (!await _templateRepository.ExistsAsync(storageDto.TemplateId, userId))
        {
            throw new InvalidOperationException($"Template with ID {storageDto.TemplateId} not found");
        }

        // Check if already in storage
        if (await _storageRepository.ExistsAsync(userId, storageDto.TemplateId))
        {
            throw new InvalidOperationException("Template is already in your storage");
        }

        var storage = new StorageTemplate
        {
            UserId = userId,
            TemplateId = storageDto.TemplateId
        };

        var createdStorage = await _storageRepository.CreateAsync(storage);
        return _mapper.Map<StorageTemplateServiceDto>(createdStorage);
    }

    public async Task<bool> RemoveFromStorageAsync(int id, int userId)
    {
        var storage = await _storageRepository.GetByIdAsync(id);
        
        if (storage == null)
            return false;

        // Only the owner can remove
        if (storage.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only remove items from your own storage");
        }

        return await _storageRepository.DeleteAsync(id);
    }

    public async Task<bool> IsInStorageAsync(int userId, int templateId)
    {
        return await _storageRepository.ExistsAsync(userId, templateId);
    }
} 
