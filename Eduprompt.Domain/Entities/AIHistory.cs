using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class AIHistory
{
    [Key]
    public int AIHistoryID { get; set; }

    [Required]
    public int UserID { get; set; }

    public int? ConversationID { get; set; }

    public int? PromptInstanceID { get; set; }

    public string? UserMessage { get; set; }

    public string? AIResponse { get; set; }

    [Required]
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    public int? ProcessingTimeMs { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Completed";

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("ConversationID")]
    public virtual Conversation? Conversation { get; set; }

    [ForeignKey("PromptInstanceID")]
    public virtual PromptInstance? PromptInstance { get; set; }
}
