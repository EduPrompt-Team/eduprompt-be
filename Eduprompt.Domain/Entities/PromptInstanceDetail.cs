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
    public string ParameterName { get; set; } = string.Empty;

    [Required]
    public string ParameterValue { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string ParameterType { get; set; } = "Text"; // e.g., 'Text', 'Number', 'Boolean', 'JSON'

    // Navigation properties
    [ForeignKey("InstanceID")]
    public virtual PromptInstance PromptInstance { get; set; } = null!;
}
