using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class PromptInstanceDetail
{
    [Key]
    public int DetailID { get; set; }

    [Required]
    public int InstanceID { get; set; }

    [Required]
    [StringLength(100)]
    public string FieldName { get; set; } = string.Empty;

    public string? FieldValue { get; set; }

    [StringLength(50)]
    public string? FieldType { get; set; } = "Text"; // 'Text', 'Number', 'Date', 'Boolean', 'JSON'

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    public string? Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey("InstanceID")]
    public virtual PromptInstance PromptInstance { get; set; } = null!;
}
