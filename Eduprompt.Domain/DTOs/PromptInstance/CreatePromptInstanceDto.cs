using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.PromptInstance;

public class CreatePromptInstanceDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int PackageId { get; set; }

    [Required]
    [StringLength(200)]
    public string PromptName { get; set; } = string.Empty;

    public string? InputJson { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Pending";
}
