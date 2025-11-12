using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.PromptInstance;

public class CreatePromptInstanceDto
{
    [Required]
    public int UserId { get; set; }

    // PackageId is optional - can be null or 0
    // If null/0 and storageId is provided, packageId will be auto-mapped from StorageTemplate
    public int? PackageId { get; set; }

    // StorageId is optional - used to auto-map packageId from StorageTemplate
    // If packageId is null/0 and storageId is provided, packageId will be resolved from StorageTemplate
    public int? StorageId { get; set; }

    [Required]
    [StringLength(200)]
    public string PromptName { get; set; } = string.Empty;

    public string? InputJson { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Pending";
}
