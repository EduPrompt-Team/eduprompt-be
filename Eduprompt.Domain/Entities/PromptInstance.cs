using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("PromptInstances")]
public partial class PromptInstance
{
    [Key]
    [Column("InstanceID")]
    public int InstanceID { get; set; }

    [Required]
    [Column("UserID")]
    public int UserID { get; set; }

    [Required]
    [Column("PackageID")]
    public int PackageID { get; set; }

    [Required]
    [StringLength(200)]
    [Column("PromptName")]
    public string PromptName { get; set; } = string.Empty;

    [Column("InputJson")]
    public string? InputJson { get; set; } // JSON string of user input

    [Column("OutputJson")]
    public string? OutputJson { get; set; } // JSON string of AI output

    [Required]
    [Column("ExecutedAt")]
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    [Column("ProcessingTimeMs")]
    public int? ProcessingTimeMs { get; set; }

    [StringLength(50)]
    [Column("Status")]
    public string? Status { get; set; } = "Completed"; // 'Pending', 'Completed', 'Failed'

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PackageID")]
    public virtual Package Package { get; set; } = null!;

    public virtual ICollection<PromptInstanceDetail> PromptInstanceDetails { get; set; } = new List<PromptInstanceDetail>();
    public virtual ICollection<AIHistory> AIHistories { get; set; } = new List<AIHistory>();
    public virtual ICollection<ExpectedOutput> ExpectedOutputs { get; set; } = new List<ExpectedOutput>();
}
