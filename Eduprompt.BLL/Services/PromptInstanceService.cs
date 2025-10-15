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

    public PromptInstanceService(IPromptInstanceRepository promptInstanceRepository)
    {
        _promptInstanceRepository = promptInstanceRepository;
    }

    public async Task<PromptInstanceDto?> GetByIdAsync(int instanceId)
    {
        var instance = await _promptInstanceRepository.GetByIdAsync(instanceId);
        return instance != null ? MapToDto(instance) : null;
    }

    public async Task<IEnumerable<PromptInstanceDto>> GetByUserIdAsync(int userId)
    {
        var instances = await _promptInstanceRepository.GetByUserIdAsync(userId);
        return instances.Select(MapToDto);
    }

    public async Task<IEnumerable<PromptInstanceDto>> GetByTemplateIdAsync(int templateId)
    {
        var instances = await _promptInstanceRepository.GetByPackageIdAsync(templateId);
        return instances.Select(MapToDto);
    }

    public async Task<PromptInstanceDto> CreateAsync(CreatePromptInstanceDto createPromptInstanceDto)
    {
        var instance = new PromptInstance
        {
            UserID = createPromptInstanceDto.UserID,
            PackageID = createPromptInstanceDto.PackageID,
            PromptName = createPromptInstanceDto.PromptName,
            InputJson = createPromptInstanceDto.InputJson,
            Status = createPromptInstanceDto.Status ?? "Pending",
            ExecutedAt = DateTime.UtcNow
        };

        var createdInstance = await _promptInstanceRepository.CreateAsync(instance);
        return MapToDto(createdInstance);
    }

    public async Task<PromptInstanceDto> UpdateAsync(int instanceId, UpdatePromptInstanceDto updateDto)
    {
        var instance = await _promptInstanceRepository.GetByIdAsync(instanceId);
        if (instance == null) return null;

        instance.PromptName = updateDto.PromptName;
        instance.InputJson = updateDto.InputJson;
        instance.OutputJson = updateDto.OutputJson;
        instance.Status = updateDto.Status ?? instance.Status;

        var updatedInstance = await _promptInstanceRepository.UpdateAsync(instance);
        return MapToDto(updatedInstance);
    }

    public async Task<IEnumerable<PromptInstanceDto>> GetByStatusAsync(string status)
    {
        var instances = await _promptInstanceRepository.GetByStatusAsync(status);
        return instances.Select(MapToDto);
    }

    public async Task<IEnumerable<PromptInstanceDto>> GetRecentInstancesAsync(int userId, int count = 10)
    {
        var instances = await _promptInstanceRepository.GetRecentInstancesAsync(userId, count);
        return instances.Select(MapToDto);
    }

    public async Task<bool> CompleteInstanceAsync(int instanceId, string outputData)
    {
        var instance = await _promptInstanceRepository.GetByIdAsync(instanceId);
        if (instance == null) return false;

        instance.OutputJson = outputData;
        instance.Status = "Completed";
        instance.ExecutedAt = DateTime.UtcNow;

        await _promptInstanceRepository.UpdateAsync(instance);
        return true;
    }

    private static PromptInstanceDto MapToDto(PromptInstance promptInstance)
    {
        return new PromptInstanceDto
        {
            InstanceID = promptInstance.InstanceID,
            UserID = promptInstance.UserID,
            PackageID = promptInstance.PackageID,
            PromptName = promptInstance.PromptName,
            InputJson = promptInstance.InputJson,
            OutputJson = promptInstance.OutputJson,
            Status = promptInstance.Status,
            ExecutedAt = promptInstance.ExecutedAt,
            ProcessingTimeMs = promptInstance.ProcessingTimeMs,
            UserName = promptInstance.User?.FullName,
            PackageName = promptInstance.Package?.PackageName
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _promptInstanceRepository.DeleteAsync(id);
    }
}













