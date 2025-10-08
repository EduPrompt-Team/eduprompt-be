using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Message;

public class CreateMessageDto
{
    [Required]
    public int ConversationID { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    [StringLength(50)]
    public string? MessageType { get; set; } = "Text";

    [StringLength(50)]
    public string? SenderType { get; set; } = "User";

    [StringLength(50)]
    public string? Status { get; set; } = "Sent";
}
