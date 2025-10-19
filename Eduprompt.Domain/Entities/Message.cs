using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("Messages")]
public partial class Message
{
    [Key]
    [Column("MessageID")]
    public int MessageID { get; set; }

    [Required]
    [Column("ConversationID")]
    public int ConversationID { get; set; }

    [Required]
    [StringLength(20)]
    [Column("SenderType")]
    public string SenderType { get; set; } = string.Empty; // 'User', 'AI', 'System'

    [Required]
    [Column("Content")]
    public string Content { get; set; } = string.Empty;

    [Required]
    [Column("SentAt")]
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    [Required]
    [Column("IsRead")]
    public bool IsRead { get; set; } = false;

    [StringLength(50)]
    [Column("Status")]
    public string? Status { get; set; } = "Sent";

    // Navigation properties
    [ForeignKey("ConversationID")]
    public virtual Conversation Conversation { get; set; } = null!;
}
