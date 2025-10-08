using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class AIHistory
{
    [Key]
    public int AIHistoryID { get; set; }

    [Required]
    public int UserID { get; set; }

    public int? PromptInstanceID { get; set; }

    public string? InputText { get; set; }

    public string? OutputText { get; set; }

    [StringLength(100)]
    public string? ModelUsed { get; set; }

    public int? TokensUsed { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal? Cost { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    public string? Status { get; set; } = "Completed";

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PromptInstanceID")]
    public virtual PromptInstance? PromptInstance { get; set; }
}
