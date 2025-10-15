using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class ExpectedOutput
{
    [Key]
    public int OutputID { get; set; }

    [Required]
    public int PromptInstanceID { get; set; }

    [Required]
    [StringLength(100)]
    public string OutputName { get; set; } = string.Empty;

    public string? ValidationRules { get; set; } // JSON string of validation rules

    public string? ExampleOutput { get; set; } // Example of expected output

    // Navigation properties
    [ForeignKey("PromptInstanceID")]
    public virtual PromptInstance PromptInstance { get; set; } = null!;

    public virtual ICollection<OutputDetail> OutputDetails { get; set; } = new List<OutputDetail>();
}
