using Eduprompt.Domain.DTOs.TemplateArchitecture;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class TemplateArchitectureService : ITemplateArchitectureService
{
    private readonly ITemplateArchitectureRepository _architectureRepository;
    // Using instance repository as originally wired; do not depend on StorageTemplate DTOs here
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
        // Ensure instance exists
        var instance = await _instanceRepository.GetByIdAsync(createDto.PromptInstanceID);
        if (instance == null) throw new ArgumentException("Prompt instance not found");

        var e = new Eduprompt.Domain.Entities.TemplateArchitecture
        {
            StorageID = 1, // Default storage template ID - should be created in database
            ArchitectureName = createDto.ArchitectureName,
            ArchitectureType = "Sequential",
            ConfigurationJson = createDto.Configuration ?? "{}"
        };

        var created = await _architectureRepository.CreateAsync(e);
        return Map(created);
    }

    public async Task<TemplateArchitectureDto> UpdateAsync(int architectureId, CreateTemplateArchitectureDto updateDto)
    {
        var architecture = await _architectureRepository.GetByIdAsync(architectureId);
        if (architecture == null) throw new KeyNotFoundException("Template architecture not found");

        architecture.ArchitectureName = updateDto.ArchitectureName;
        architecture.ArchitectureType = "Sequential";
        architecture.ConfigurationJson = updateDto.Configuration;

        var updatedArchitecture = await _architectureRepository.UpdateAsync(architecture);
        return Map(updatedArchitecture);
    }

    private static TemplateArchitectureDto Map(Eduprompt.Domain.Entities.TemplateArchitecture e)
    {
        return new TemplateArchitectureDto
        {
            ArchitectureID = e.ArchitectureID,
            PromptInstanceID = 0,
            ArchitectureName = e.ArchitectureName,
            Configuration = e.ConfigurationJson,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            Status = "Active"
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _architectureRepository.DeleteAsync(id);
    }
}










