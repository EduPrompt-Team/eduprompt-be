using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("AIHistories")]
public partial class AIHistory
{
    [Key]
    [Column("AIHistoryID")]
    public int AIHistoryID { get; set; }

    [Required]
    [Column("UserID")]
    public int UserID { get; set; }

    [Column("ConversationID")]
    public int? ConversationID { get; set; }

    [Column("PromptInstanceID")]
    public int? PromptInstanceID { get; set; }

    [Column("UserMessage")]
    public string? UserMessage { get; set; }

    [Column("AIResponse")]
    public string? AIResponse { get; set; }

    [Required]
    [Column("ExecutedAt")]
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    [Column("ProcessingTimeMs")]
    public int? ProcessingTimeMs { get; set; }

    [StringLength(50)]
    [Column("Status")]
    public string? Status { get; set; } = "Completed";

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("ConversationID")]
    public virtual Conversation? Conversation { get; set; }

    [ForeignKey("PromptInstanceID")]
    public virtual PromptInstance? PromptInstance { get; set; }
}
