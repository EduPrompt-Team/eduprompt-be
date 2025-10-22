namespace Eduprompt.Domain.DTOs.TemplateArchitecture;

public class TemplateArchitectureDto
{
    public int ArchitectureId { get; set; }
    public int PromptInstanceId { get; set; }
    public int StorageId { get; set; }
    public string ArchitectureName { get; set; } = string.Empty;
    public string? ArchitectureType { get; set; }
    public string? Configuration { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
}
