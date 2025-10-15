using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Message;

public class CreateMessageDto
{
    [Required]
    public int ConversationID { get; set; }

    [Required]
    [StringLength(20)]
    public string SenderType { get; set; } = "User";

    [Required]
    public string Content { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    [StringLength(50)]
    public string? Status { get; set; } = "Sent";
}
