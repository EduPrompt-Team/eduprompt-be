namespace Eduprompt.Domain.DTOs.PromptInstance;

public class PromptInstanceDto
{
    public int InstanceID { get; set; }
    public int UserID { get; set; }
    public int TemplateID { get; set; }
    public string InstanceName { get; set; } = string.Empty;
    public string? InputData { get; set; }
    public string? OutputData { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? TemplateName { get; set; }
    public string? UserName { get; set; }
}
