using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("ExpectedOutputs")]
public partial class ExpectedOutput
{
    [Key]
    [Column("OutputID")]
    public int OutputID { get; set; }

    [Required]
    [Column("PromptInstanceID")]
    public int PromptInstanceID { get; set; }

    [Required]
    [StringLength(100)]
    [Column("OutputName")]
    public string OutputName { get; set; } = string.Empty;

    [Column("ValidationRules")]
    public string? ValidationRules { get; set; } // JSON string of validation rules

    [Column("ExampleOutput")]
    public string? ExampleOutput { get; set; } // Example of expected output

    // Navigation properties
    [ForeignKey("PromptInstanceID")]
    public virtual PromptInstance PromptInstance { get; set; } = null!;

    public virtual ICollection<OutputDetail> OutputDetails { get; set; } = new List<OutputDetail>();
}
