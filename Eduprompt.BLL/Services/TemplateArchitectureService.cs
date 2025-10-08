using Eduprompt.Domain.DTOs.TemplateArchitecture;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class TemplateArchitectureService : ITemplateArchitectureService
{
    private readonly ITemplateArchitectureRepository _architectureRepository;
    private readonly IPromptInstanceRepository _instanceRepository;

    public TemplateArchitectureService(
        ITemplateArchitectureRepository architectureRepository,
        IPromptInstanceRepository instanceRepository)
    {
        _architectureRepository = architectureRepository;
        _instanceRepository = instanceRepository;
    }

    public async Task<TemplateArchitectureDto?> GetByIdAsync(int architectureId)
    {
        var e = await _architectureRepository.GetByIdAsync(architectureId);
        return e == null ? null : Map(e);
    }

    public async Task<IEnumerable<TemplateArchitectureDto>> GetByPromptInstanceIdAsync(int promptInstanceId)
    {
        var list = await _architectureRepository.GetByInstanceIdAsync(promptInstanceId);
        return list.Select(Map);
    }

    public async Task<TemplateArchitectureDto> CreateAsync(CreateTemplateArchitectureDto createDto)
    {
        // Ensure instance exists (mapping PromptInstanceID -> underlying foreign key as per model)
        var instance = await _instanceRepository.GetByIdAsync(createDto.PromptInstanceID);
        if (instance == null) throw new ArgumentException("Prompt instance not found");

        var e = new Eduprompt.Domain.Entities.TemplateArchitecture
        {
            TemplateID = createDto.PromptInstanceID, // note: entity currently references TemplateID
            ArchitectureName = createDto.ArchitectureName,
            ArchitectureType = "Sequential",
            Configuration = createDto.Configuration,
            Status = createDto.Status,
            CreatedDate = DateTime.UtcNow
        };

        var created = await _architectureRepository.CreateAsync(e);
        return Map(created);
    }

    public async Task<TemplateArchitectureDto> UpdateAsync(int architectureId, CreateTemplateArchitectureDto updateDto)
    {
        var e = await _architectureRepository.GetByIdAsync(architectureId);
        if (e == null) throw new KeyNotFoundException("Template architecture not found");

        if (updateDto.PromptInstanceID != 0) e.TemplateID = updateDto.PromptInstanceID;
        if (!string.IsNullOrEmpty(updateDto.ArchitectureName)) e.ArchitectureName = updateDto.ArchitectureName;
        // Entity does not have Description; ignore mapping from DTO to entity here
        if (updateDto.Configuration != null) e.Configuration = updateDto.Configuration;
        if (updateDto.Status != null) e.Status = updateDto.Status;

        var updated = await _architectureRepository.UpdateAsync(e);
        return Map(updated);
    }

    public async Task<bool> DeleteAsync(int architectureId)
    {
        return await _architectureRepository.DeleteAsync(architectureId);
    }

    private static TemplateArchitectureDto Map(Eduprompt.Domain.Entities.TemplateArchitecture e)
    {
        return new TemplateArchitectureDto
        {
            ArchitectureID = e.ArchitectureID,
            PromptInstanceID = e.TemplateID,
            ArchitectureName = e.ArchitectureName,
            // Entity does not have Description field; DTO Description left null
            Configuration = e.Configuration,
            CreatedDate = e.CreatedDate,
            UpdatedDate = e.UpdatedDate,
            Status = e.Status,
            InstanceName = null
        };
    }
}


