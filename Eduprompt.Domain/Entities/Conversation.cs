using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("Conversations")]
public partial class Conversation
{
    [Key]
    [Column("ConversationID")]
    public int ConversationID { get; set; }

    [Required]
    [Column("UserID")]
    public int UserID { get; set; }

    [StringLength(200)]
    [Column("Title")]
    public string? Title { get; set; }

    [Required]
    [Column("StartedAt")]
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    [Column("LastActivity")]
    public DateTime? LastActivity { get; set; }

    [StringLength(50)]
    [Column("Status")]
    public string? Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    public virtual ICollection<AIHistory> AIHistories { get; set; } = new List<AIHistory>();
}
