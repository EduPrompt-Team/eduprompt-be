namespace Eduprompt.Domain.DTOs.TemplateArchitecture;

public class TemplateArchitectureDto
{
    public int ArchitectureID { get; set; }
    public int PromptInstanceID { get; set; }
    public string ArchitectureName { get; set; } = string.Empty;
    public string? Configuration { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
}
