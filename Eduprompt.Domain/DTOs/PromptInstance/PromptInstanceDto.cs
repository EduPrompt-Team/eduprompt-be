namespace Eduprompt.Domain.DTOs.PromptInstance;

public class PromptInstanceDto
{
    public int InstanceID { get; set; }
    public int UserID { get; set; }
    public int PackageID { get; set; }
    public string PromptName { get; set; } = string.Empty;
    public string? InputJson { get; set; }
    public string? OutputJson { get; set; }
    public DateTime ExecutedAt { get; set; }
    public int? ProcessingTimeMs { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public string? PackageName { get; set; }
}
