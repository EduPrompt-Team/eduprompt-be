using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public class ExpectedOutput
{
    [Key]
    public int OutputId { get; set; }

    [Required]
    public int InstanceID { get; set; }

    [Required]
    [StringLength(100)]
    public string OutputName { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Active";

    [ForeignKey("InstanceID")]
    public virtual PromptInstance PromptInstance { get; set; } = null!;

    public virtual ICollection<OutputDetail> OutputDetails { get; set; } = new List<OutputDetail>();
}


