namespace Eduprompt.Domain.DTOs.PromptInstance;

public class PromptInstanceDto
{
    public int InstanceId { get; set; }
    public int UserId { get; set; }
    public int? PackageId { get; set; } // Nullable - can be null if created from StorageTemplate without package
    public int? StorageId { get; set; } // Optional - storage template ID if created from template
    public string PromptName { get; set; } = string.Empty;
    public string? InputJson { get; set; }
    public string? OutputJson { get; set; }
    public DateTime ExecutedAt { get; set; }
    public int? ProcessingTimeMs { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public string? PackageName { get; set; }
}
