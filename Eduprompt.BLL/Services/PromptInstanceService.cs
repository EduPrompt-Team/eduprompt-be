using Eduprompt.Domain.DTOs.PromptInstance;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

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
        var instances = await _promptInstanceRepository.GetByTemplateIdAsync(templateId);
        return instances.Select(MapToDto);
    }

    public async Task<PromptInstanceDto> CreateAsync(CreatePromptInstanceDto createPromptInstanceDto)
    {
        var instance = new PromptInstance
        {
            UserID = createPromptInstanceDto.UserID,
            TemplateID = createPromptInstanceDto.TemplateID,
            InstanceName = createPromptInstanceDto.InstanceName,
            InputData = createPromptInstanceDto.InputData,
            Status = createPromptInstanceDto.Status,
            CreatedDate = DateTime.UtcNow
        };

        var createdInstance = await _promptInstanceRepository.CreateAsync(instance);
        return MapToDto(createdInstance);
    }

    public async Task<PromptInstanceDto> UpdateAsync(int instanceId, UpdatePromptInstanceDto updatePromptInstanceDto)
    {
        var instance = await _promptInstanceRepository.GetByIdAsync(instanceId);
        if (instance == null)
            throw new ArgumentException("Prompt instance not found");

        if (!string.IsNullOrEmpty(updatePromptInstanceDto.InstanceName))
            instance.InstanceName = updatePromptInstanceDto.InstanceName;

        if (updatePromptInstanceDto.InputData != null)
            instance.InputData = updatePromptInstanceDto.InputData;

        if (updatePromptInstanceDto.OutputData != null)
            instance.OutputData = updatePromptInstanceDto.OutputData;

        if (!string.IsNullOrEmpty(updatePromptInstanceDto.Status))
            instance.Status = updatePromptInstanceDto.Status;

        instance.UpdatedDate = DateTime.UtcNow;

        var updatedInstance = await _promptInstanceRepository.UpdateAsync(instance);
        return MapToDto(updatedInstance);
    }

    public async Task<bool> DeleteAsync(int instanceId)
    {
        return await _promptInstanceRepository.DeleteAsync(instanceId);
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

        instance.OutputData = outputData;
        instance.Status = "Completed";
        instance.CompletedDate = DateTime.UtcNow;
        instance.UpdatedDate = DateTime.UtcNow;

        await _promptInstanceRepository.UpdateAsync(instance);
        return true;
    }

    private static PromptInstanceDto MapToDto(PromptInstance instance)
    {
        return new PromptInstanceDto
        {
            InstanceID = instance.InstanceID,
            UserID = instance.UserID,
            TemplateID = instance.TemplateID,
            InstanceName = instance.InstanceName,
            InputData = instance.InputData,
            OutputData = instance.OutputData,
            Status = instance.Status,
            CreatedDate = instance.CreatedDate,
            UpdatedDate = instance.UpdatedDate,
            CompletedDate = instance.CompletedDate,
            TemplateName = "Template " + instance.TemplateID, // StorageTemplate doesn't have TemplateName property
            UserName = instance.User?.FullName
        };
    }
}
