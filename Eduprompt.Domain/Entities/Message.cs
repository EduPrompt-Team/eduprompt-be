using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class Message
{
    [Key]
    public int MessageID { get; set; }

    [Required]
    public int ConversationID { get; set; }

    [Required]
    [StringLength(20)]
    public string SenderType { get; set; } = string.Empty; // 'User', 'AI', 'System'

    [Required]
    public string Content { get; set; } = string.Empty;

    [StringLength(50)]
    public string? MessageType { get; set; } = "Text"; // 'Text', 'Image', 'File', 'Prompt'

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    public string? Status { get; set; } = "Sent";

    // Navigation properties
    [ForeignKey("ConversationID")]
    public virtual Conversation Conversation { get; set; } = null!;
}
