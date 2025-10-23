using Eduprompt.Domain.DTOs.TemplateArchitecture;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class TemplateArchitectureService : ITemplateArchitectureService
{
    private readonly ITemplateArchitectureRepository _architectureRepository;

    public TemplateArchitectureService(ITemplateArchitectureRepository architectureRepository)
    {
        _architectureRepository = architectureRepository;
    }

    public async Task<TemplateArchitectureDto?> GetByIdAsync(int ArchitectureId)
    {
        var e = await _architectureRepository.GetByIdAsync(ArchitectureId);
        return e == null ? null : Map(e);
    }

    public async Task<IEnumerable<TemplateArchitectureDto>> GetByInstanceIdAsync(int InstanceId)
    {
        var list = await _architectureRepository.GetByInstanceIdAsync(InstanceId);
        return list.Select(Map);
    }

    public async Task<IEnumerable<TemplateArchitectureDto>> GetByPromptInstanceIdAsync(int PromptInstanceId)
    {
        var list = await _architectureRepository.GetByInstanceIdAsync(PromptInstanceId);
        return list.Select(Map);
    }

    public async Task<TemplateArchitectureDto> CreateAsync(CreateTemplateArchitectureDto createDto)
    {
        var e = new Eduprompt.Domain.Entities.TemplateArchitecture
        {
            StorageId = createDto.StorageId,
            ArchitectureName = createDto.ArchitectureName,
            ArchitectureType = createDto.ArchitectureType ?? "Sequential",
            ConfigurationJson = createDto.Configuration ?? "{}"
        };

        var created = await _architectureRepository.CreateAsync(e);
        return Map(created);
    }

    public async Task<TemplateArchitectureDto> UpdateAsync(int ArchitectureId, CreateTemplateArchitectureDto updateDto)
    {
        var architecture = await _architectureRepository.GetByIdAsync(ArchitectureId);
        if (architecture == null) throw new KeyNotFoundException("Template architecture not found");

        architecture.ArchitectureName = updateDto.ArchitectureName;
        architecture.ArchitectureType = updateDto.ArchitectureType ?? architecture.ArchitectureType;
        architecture.ConfigurationJson = updateDto.Configuration ?? architecture.ConfigurationJson;

        var updatedArchitecture = await _architectureRepository.UpdateAsync(architecture);
        return Map(updatedArchitecture);
    }

    private static TemplateArchitectureDto Map(Eduprompt.Domain.Entities.TemplateArchitecture e)
    {
        return new TemplateArchitectureDto
        {
            ArchitectureId = e.ArchitectureId,
            PromptInstanceId = e.StorageId, // Map StorageId to PromptInstanceId
            StorageId = e.StorageId,
            ArchitectureName = e.ArchitectureName,
            ArchitectureType = e.ArchitectureType,
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










