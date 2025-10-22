using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.PromptInstanceDetail;

public class CreatePromptInstanceDetailDto
{
    [Required]
    public int PromptInstanceId { get; set; }

    [Required]
    [StringLength(100)]
    public string FieldName { get; set; } = string.Empty;

    public string? FieldValue { get; set; }

    [StringLength(50)]
    public string? FieldType { get; set; } = "Text";

    public int? OrderIndex { get; set; }
}
