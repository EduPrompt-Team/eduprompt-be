using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.PromptInstance;

public class UpdatePromptInstanceDto
{
    [StringLength(200)]
    public string? PromptName { get; set; }

    public string? InputJson { get; set; }

    public string? OutputJson { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }
}
