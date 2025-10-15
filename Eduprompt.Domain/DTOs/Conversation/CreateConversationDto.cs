using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Conversation;

public class CreateConversationDto
{
    [Required]
    public int UserID { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Active";
}
