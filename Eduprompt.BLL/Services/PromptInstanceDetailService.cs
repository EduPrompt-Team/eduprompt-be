using Eduprompt.Domain.DTOs.PromptInstanceDetail;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class PromptInstanceDetailService : IPromptInstanceDetailService
{
    private readonly IPromptInstanceDetailRepository _promptInstanceDetailRepository;

    public PromptInstanceDetailService(IPromptInstanceDetailRepository promptInstanceDetailRepository)
    {
        _promptInstanceDetailRepository = promptInstanceDetailRepository;
    }

    public async Task<PromptInstanceDetailDto?> GetByIdAsync(int detailId)
    {
        var detail = await _promptInstanceDetailRepository.GetByIdAsync(detailId);
        return detail != null ? MapToDto(detail) : null;
    }

    public async Task<IEnumerable<PromptInstanceDetailDto>> GetByInstanceIdAsync(int instanceId)
    {
        var details = await _promptInstanceDetailRepository.GetByInstanceIdAsync(instanceId);
        return details.Select(MapToDto);
    }

    public async Task<PromptInstanceDetailDto> CreateAsync(CreatePromptInstanceDetailDto createDto)
    {
        var detail = new PromptInstanceDetail
        {
            InstanceID = createDto.InstanceID,
            ParameterName = createDto.FieldName,
            ParameterValue = createDto.FieldValue ?? string.Empty,
            ParameterType = createDto.FieldType ?? "Text"
        };

        var createdDetail = await _promptInstanceDetailRepository.CreateAsync(detail);
        return MapToDto(createdDetail);
    }

    public async Task<PromptInstanceDetailDto> UpdateAsync(int detailId, CreatePromptInstanceDetailDto updateDto)
    {
        var detail = await _promptInstanceDetailRepository.GetByIdAsync(detailId);
        if (detail == null) throw new KeyNotFoundException("Prompt instance detail not found");

        detail.ParameterName = updateDto.FieldName;
        detail.ParameterValue = updateDto.FieldValue ?? string.Empty;
        detail.ParameterType = updateDto.FieldType ?? detail.ParameterType;

        var updatedDetail = await _promptInstanceDetailRepository.UpdateAsync(detail);
        return MapToDto(updatedDetail);
    }

    public async Task<bool> DeleteAsync(int detailId)
    {
        return await _promptInstanceDetailRepository.DeleteAsync(detailId);
    }

    private static PromptInstanceDetailDto MapToDto(PromptInstanceDetail detail)
    {
        return new PromptInstanceDetailDto
        {
            DetailID = detail.DetailID,
            InstanceID = detail.InstanceID,
            FieldName = detail.ParameterName,
            FieldValue = detail.ParameterValue,
            FieldType = detail.ParameterType,
            OrderIndex = 0, // Default order index
            InstanceName = detail.PromptInstance?.PromptName,
            UpdatedDate = DateTime.UtcNow
        };
    }
}