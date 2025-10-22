namespace Eduprompt.Domain.DTOs.PromptInstanceDetail;

public class PromptInstanceDetailDto
{
    public int DetailId { get; set; }
    public int PromptInstanceId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? FieldValue { get; set; }
    public string? FieldType { get; set; }
    public int? OrderIndex { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? PromptName { get; set; }
}
