namespace Eduprompt.Domain.DTOs.Message;

public class MessageDto
{
    public int MessageId { get; set; }
    public int ConversationId { get; set; }
    public string SenderType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
    public string? Status { get; set; }
    public string? ConversationTitle { get; set; }
}
