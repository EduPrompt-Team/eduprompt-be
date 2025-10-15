using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class PromptInstance
{
    [Key]
    public int InstanceID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    public int PackageID { get; set; }

    [Required]
    [StringLength(200)]
    public string PromptName { get; set; } = string.Empty;

    public string? InputJson { get; set; } // JSON string of user input

    public string? OutputJson { get; set; } // JSON string of AI output

    [Required]
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    public int? ProcessingTimeMs { get; set; }

    [StringLength(50)]
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
