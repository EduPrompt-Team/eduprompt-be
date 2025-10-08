using Eduprompt.Domain.DTOs.PromptInstanceDetail;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class PromptInstanceDetailService : IPromptInstanceDetailService
{
    private readonly IPromptInstanceDetailRepository _detailRepository;
    private readonly IPromptInstanceRepository _instanceRepository;

    public PromptInstanceDetailService(
        IPromptInstanceDetailRepository detailRepository,
        IPromptInstanceRepository instanceRepository)
    {
        _detailRepository = detailRepository;
        _instanceRepository = instanceRepository;
    }

    public async Task<PromptInstanceDetailDto?> GetByIdAsync(int detailId)
    {
        var e = await _detailRepository.GetByIdAsync(detailId);
        return e == null ? null : Map(e);
    }

    public async Task<IEnumerable<PromptInstanceDetailDto>> GetByInstanceIdAsync(int instanceId)
    {
        var list = await _detailRepository.GetByInstanceIdAsync(instanceId);
        return list.Select(Map);
    }

    public async Task<PromptInstanceDetailDto> CreateAsync(CreatePromptInstanceDetailDto createDto)
    {
        var instance = await _instanceRepository.GetByIdAsync(createDto.InstanceID);
        if (instance == null) throw new ArgumentException("Prompt instance not found");

        var e = new Eduprompt.Domain.Entities.PromptInstanceDetail
        {
            InstanceID = createDto.InstanceID,
            FieldName = createDto.FieldName,
            FieldValue = createDto.FieldValue,
            FieldType = createDto.FieldType,
            CreatedDate = DateTime.UtcNow,
            Status = "Active"
        };

        var created = await _detailRepository.CreateAsync(e);
        return Map(created);
    }

    public async Task<PromptInstanceDetailDto> UpdateAsync(int detailId, CreatePromptInstanceDetailDto updateDto)
    {
        var e = await _detailRepository.GetByIdAsync(detailId);
        if (e == null) throw new KeyNotFoundException("Prompt instance detail not found");

        if (updateDto.InstanceID != 0) e.InstanceID = updateDto.InstanceID;
        if (!string.IsNullOrEmpty(updateDto.FieldName)) e.FieldName = updateDto.FieldName;
        if (updateDto.FieldValue != null) e.FieldValue = updateDto.FieldValue;
        if (updateDto.FieldType != null) e.FieldType = updateDto.FieldType;

        var updated = await _detailRepository.UpdateAsync(e);
        return Map(updated);
    }

    public async Task<bool> DeleteAsync(int detailId)
    {
        return await _detailRepository.DeleteAsync(detailId);
    }

    private static PromptInstanceDetailDto Map(Eduprompt.Domain.Entities.PromptInstanceDetail e)
    {
        return new PromptInstanceDetailDto
        {
            DetailID = e.DetailID,
            InstanceID = e.InstanceID,
            FieldName = e.FieldName,
            FieldValue = e.FieldValue,
            FieldType = e.FieldType,
            CreatedDate = e.CreatedDate,
            UpdatedDate = null,
            InstanceName = e.PromptInstance?.InstanceName
        };
    }
}


