using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Conversation;

public class CreateConversationDto
{
    [Required]
    public int UserID { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    // [StringLength(500)]
    // public string? Description { get; set; } // Removed - Conversation entity doesn't have Description property

    [StringLength(50)]
    public string? Status { get; set; } = "Active";
}
