using Eduprompt.Domain.DTOs.PromptInstance;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Eduprompt.BLL.Services;

public class PromptInstanceService : IPromptInstanceService
{
    private readonly IPromptInstanceRepository _promptInstanceRepository;
    private readonly IStorageTemplateRepository _storageTemplateRepository;
    private readonly IPackageRepository _packageRepository;

    public PromptInstanceService(
        IPromptInstanceRepository promptInstanceRepository,
        IStorageTemplateRepository storageTemplateRepository,
        IPackageRepository packageRepository)
    {
        _promptInstanceRepository = promptInstanceRepository;
        _storageTemplateRepository = storageTemplateRepository;
        _packageRepository = packageRepository;
    }

    public async Task<PromptInstanceDto?> GetByIdAsync(int InstanceId)
    {
        var instance = await _promptInstanceRepository.GetByIdAsync(InstanceId);
        return instance != null ? MapToDto(instance) : null;
    }

    public async Task<IEnumerable<PromptInstanceDto>> GetByUserIdAsync(int UserId)
    {
        var instances = await _promptInstanceRepository.GetByUserIdAsync(UserId);
        return instances.Select(MapToDto);
    }

    public async Task<IEnumerable<PromptInstanceDto>> GetByTemplateIdAsync(int templateId)
    {
        var instances = await _promptInstanceRepository.GetByTemplateIdAsync(templateId);
        return instances.Select(MapToDto);
    }

    public async Task<IEnumerable<PromptInstanceDto>> GetByStorageIdAsync(int storageId)
    {
        // Get StorageTemplate to find its PackageId
        var storageTemplate = await _storageTemplateRepository.GetByIdAsync(storageId);
        if (storageTemplate == null)
        {
            return Enumerable.Empty<PromptInstanceDto>();
        }

        // If StorageTemplate has PackageId, get instances by PackageId
        // Note: This returns ALL instances with matching PackageId, not filtered by UserId
        if (storageTemplate.PackageId > 0)
        {
            var instances = await _promptInstanceRepository.GetByTemplateIdAsync(storageTemplate.PackageId);
            return instances.Select(MapToDto);
        }

        // If StorageTemplate doesn't have PackageId, return empty (no instances can be linked)
        return Enumerable.Empty<PromptInstanceDto>();
    }

    public async Task<IEnumerable<PromptInstanceDto>> GetByStorageIdAndUserIdAsync(int storageId, int userId)
    {
        // Get StorageTemplate to find its PackageId
        var storageTemplate = await _storageTemplateRepository.GetByIdAsync(storageId);
        if (storageTemplate == null)
        {
            return Enumerable.Empty<PromptInstanceDto>();
        }

        // If StorageTemplate has PackageId, get instances by PackageId AND UserId
        if (storageTemplate.PackageId > 0)
        {
            // Get all instances with matching PackageId, then filter by UserId
            var allInstances = await _promptInstanceRepository.GetByTemplateIdAsync(storageTemplate.PackageId);
            var userInstances = allInstances.Where(i => i.UserId == userId);
            return userInstances.Select(MapToDto);
        }

        // If StorageTemplate doesn't have PackageId, return empty (no instances can be linked)
        return Enumerable.Empty<PromptInstanceDto>();
    }

    public async Task<PromptInstanceDto> CreateAsync(CreatePromptInstanceDto createPromptInstanceDto)
    {
        // Resolve PackageId:
        // 1. If PackageId is provided and > 0, use it (validate it exists)
        // 2. If PackageId is null/0 and StorageId is provided, get PackageId from StorageTemplate
        // 3. If both are null/0, PackageId remains null (allowed for instances without package)
        
        int? packageId = null;

        // Option 1: PackageId is provided and > 0
        if (createPromptInstanceDto.PackageId.HasValue && createPromptInstanceDto.PackageId.Value > 0)
        {
            // Validate package exists
            var package = await _packageRepository.GetByIdAsync(createPromptInstanceDto.PackageId.Value);
            if (package == null)
            {
                throw new InvalidOperationException($"Package with ID {createPromptInstanceDto.PackageId.Value} not found");
            }
            packageId = createPromptInstanceDto.PackageId.Value;
        }
        // Option 2: PackageId is null/0, but StorageId is provided
        else if (createPromptInstanceDto.StorageId.HasValue && createPromptInstanceDto.StorageId.Value > 0)
        {
            // Get PackageId from StorageTemplate
            var storageTemplate = await _storageTemplateRepository.GetByIdAsync(createPromptInstanceDto.StorageId.Value);
            if (storageTemplate == null)
            {
                throw new InvalidOperationException($"StorageTemplate with ID {createPromptInstanceDto.StorageId.Value} not found");
            }
            
            // If StorageTemplate has PackageId, use it
            if (storageTemplate.PackageId > 0)
            {
                // Validate package exists
                var package = await _packageRepository.GetByIdAsync(storageTemplate.PackageId);
                if (package == null)
                {
                    throw new InvalidOperationException($"Package with ID {storageTemplate.PackageId} from StorageTemplate not found");
                }
                packageId = storageTemplate.PackageId;
            }
            // If StorageTemplate doesn't have PackageId, packageId remains null (allowed)
        }
        // Option 3: Both are null/0 - packageId remains null (allowed for instances without package)

        var instance = new PromptInstance
        {
            UserId = createPromptInstanceDto.UserId,
            PackageId = packageId ?? 0, // Use 0 as sentinel for null (DbContext will convert to NULL in DB)
            PromptName = createPromptInstanceDto.PromptName,
            InputJson = createPromptInstanceDto.InputJson,
            Status = createPromptInstanceDto.Status ?? "Pending",
            ExecutedAt = DateTime.UtcNow
        };

        var createdInstance = await _promptInstanceRepository.CreateAsync(instance);
        return MapToDto(createdInstance);
    }

    public async Task<PromptInstanceDto> UpdateAsync(int InstanceId, UpdatePromptInstanceDto updateDto)
    {
        var instance = await _promptInstanceRepository.GetByIdAsync(InstanceId);
        if (instance == null) throw new KeyNotFoundException("Prompt instance not found");

        instance.PromptName = updateDto.PromptName ?? instance.PromptName;
        instance.InputJson = updateDto.InputJson ?? instance.InputJson;
        instance.OutputJson = updateDto.OutputJson ?? instance.OutputJson;
        instance.Status = updateDto.Status ?? instance.Status;

        var updatedInstance = await _promptInstanceRepository.UpdateAsync(instance);
        return MapToDto(updatedInstance);
    }

    public async Task<IEnumerable<PromptInstanceDto>> GetByStatusAsync(string status)
    {
        var instances = await _promptInstanceRepository.GetAllAsync();
        return instances
            .Where(i => i.Status == status)
            .Select(MapToDto);
    }

    public async Task<IEnumerable<PromptInstanceDto>> GetRecentInstancesAsync(int UserId, int count = 10)
    {
        var instances = await _promptInstanceRepository.GetByUserIdAsync(UserId);
        return instances.Take(count).Select(MapToDto);
    }

    public async Task<bool> CompleteInstanceAsync(int InstanceId, string outputData)
    {
        // Legacy method - for backward compatibility
        var instance = await _promptInstanceRepository.GetByIdAsync(InstanceId);
        if (instance == null) return false;

        instance.OutputJson = outputData;
        instance.Status = "Completed";
        instance.ExecutedAt = DateTime.UtcNow;

        await _promptInstanceRepository.UpdateAsync(instance);
        return true;
    }

    public async Task<PromptInstanceDto> CompleteAsync(int InstanceId, CompletePromptInstanceDto completeDto)
    {
        var instance = await _promptInstanceRepository.GetByIdAsync(InstanceId);
        if (instance == null)
        {
            throw new KeyNotFoundException($"PromptInstance with ID {InstanceId} not found");
        }

        // Update OutputJson if provided (including empty string - allow clearing)
        // Use null check instead of IsNullOrEmpty to allow empty strings
        if (completeDto.OutputJson != null)
        {
            instance.OutputJson = completeDto.OutputJson;
        }

        // Update Status if provided
        if (!string.IsNullOrEmpty(completeDto.Status))
        {
            instance.Status = completeDto.Status;
        }
        else
        {
            instance.Status = "Completed"; // Default to Completed
        }

        // Update ProcessingTimeMs if provided
        if (completeDto.ProcessingTimeMs.HasValue)
        {
            instance.ProcessingTimeMs = completeDto.ProcessingTimeMs.Value;
        }

        // Update ExecutedAt to current time
        instance.ExecutedAt = DateTime.UtcNow;

        var updatedInstance = await _promptInstanceRepository.UpdateAsync(instance);
        return MapToDto(updatedInstance);
    }

    private static PromptInstanceDto MapToDto(PromptInstance promptInstance)
    {
        return new PromptInstanceDto
        {
            InstanceId = promptInstance.InstanceId,
            UserId = promptInstance.UserId,
            PackageId = promptInstance.PackageId == 0 ? null : promptInstance.PackageId, // Convert 0 (sentinel) to null
            StorageId = null, // TODO: Add StorageId to entity if needed
            PromptName = promptInstance.PromptName,
            InputJson = promptInstance.InputJson,
            OutputJson = promptInstance.OutputJson,
            Status = promptInstance.Status,
            ExecutedAt = promptInstance.ExecutedAt,
            ProcessingTimeMs = promptInstance.ProcessingTimeMs,
            UserName = promptInstance.User?.FullName ?? "Unknown User",
            PackageName = promptInstance.Package?.PackageName ?? (promptInstance.PackageId == 0 ? null : "Unknown Package")
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _promptInstanceRepository.DeleteAsync(id);
    }
}













