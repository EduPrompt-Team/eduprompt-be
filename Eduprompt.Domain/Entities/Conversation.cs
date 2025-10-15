using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class Conversation
{
    [Key]
    public int ConversationID { get; set; }

    [Required]
    public int UserID { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [Required]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastActivity { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    public virtual ICollection<AIHistory> AIHistories { get; set; } = new List<AIHistory>();
}
