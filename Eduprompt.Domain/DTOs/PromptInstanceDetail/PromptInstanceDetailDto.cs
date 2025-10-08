namespace Eduprompt.Domain.DTOs.PromptInstanceDetail;

public class PromptInstanceDetailDto
{
    public int DetailID { get; set; }
    public int InstanceID { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? FieldValue { get; set; }
    public string? FieldType { get; set; }
    public int? OrderIndex { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? InstanceName { get; set; }
}
