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

    public async Task<PromptInstanceDto?> GetByIdAsync(int InstanceId)
    {
        var instance = await _promptInstanceRepository.GetByIdAsync(InstanceId);
        return instance != null ? MapToDto(instance) : null;
    }

    public Task<IEnumerable<PromptInstanceDto>> GetByUserIdAsync(int UserId)
    {
        return Task.FromResult(Enumerable.Empty<PromptInstanceDto>());
    }

    public async Task<IEnumerable<PromptInstanceDto>> GetByTemplateIdAsync(int templateId)
    {
        var instances = await _promptInstanceRepository.GetByTemplateIdAsync(templateId);
        return instances.Select(MapToDto);
    }

    public async Task<PromptInstanceDto> CreateAsync(CreatePromptInstanceDto createPromptInstanceDto)
    {
        var instance = new PromptInstance
        {
            UserId = createPromptInstanceDto.UserId,
            PackageId = createPromptInstanceDto.PackageId,
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
        var instance = await _promptInstanceRepository.GetByIdAsync(InstanceId);
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
            InstanceId = promptInstance.InstanceId,
            UserId = promptInstance.UserId,
            PackageId = promptInstance.PackageId,
            PromptName = promptInstance.PromptName,
            InputJson = promptInstance.InputJson,
            OutputJson = promptInstance.OutputJson,
            Status = promptInstance.Status,
            ExecutedAt = promptInstance.ExecutedAt,
            ProcessingTimeMs = promptInstance.ProcessingTimeMs,
            UserName = promptInstance.User?.FullName ?? "Unknown User",
            PackageName = promptInstance.Package?.PackageName ?? "Unknown Package"
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _promptInstanceRepository.DeleteAsync(id);
    }
}













