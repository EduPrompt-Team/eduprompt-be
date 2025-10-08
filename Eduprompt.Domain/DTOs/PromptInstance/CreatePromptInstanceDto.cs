using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.PromptInstance;

public class CreatePromptInstanceDto
{
    [Required]
    public int UserID { get; set; }

    [Required]
    public int TemplateID { get; set; }

    [Required]
    [StringLength(200)]
    public string InstanceName { get; set; } = string.Empty;

    public string? InputData { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Draft";
}
