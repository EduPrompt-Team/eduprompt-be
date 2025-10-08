using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.PromptInstance;

public class UpdatePromptInstanceDto
{
    [StringLength(200)]
    public string? InstanceName { get; set; }

    public string? InputData { get; set; }

    public string? OutputData { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }
}
