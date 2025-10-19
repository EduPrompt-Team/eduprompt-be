using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("PromptInstanceDetails")]
public partial class PromptInstanceDetail
{
    [Key]
    [Column("DetailID")]
    public int DetailID { get; set; }

    [Required]
    [Column("InstanceID")]
    public int InstanceID { get; set; }

    [Required]
    [StringLength(100)]
    [Column("ParameterName")]
    public string ParameterName { get; set; } = string.Empty;

    [Required]
    [Column("ParameterValue")]
    public string ParameterValue { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Column("ParameterType")]
    public string ParameterType { get; set; } = "Text"; // e.g., 'Text', 'Number', 'Boolean', 'JSON'

    // Navigation properties
    [ForeignKey("InstanceID")]
    public virtual PromptInstance PromptInstance { get; set; } = null!;
}
