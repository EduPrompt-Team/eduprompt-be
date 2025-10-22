using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.TemplateArchitecture;

public class CreateTemplateArchitectureDto
{
    [Required]
    public int PromptInstanceId { get; set; }

    [Required]
    public int StorageId { get; set; }

    [Required]
    [StringLength(100)]
    public string ArchitectureName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? ArchitectureType { get; set; } = "Sequential";

    [StringLength(500)]
    public string? Description { get; set; }

    public string? Configuration { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Active";
}
