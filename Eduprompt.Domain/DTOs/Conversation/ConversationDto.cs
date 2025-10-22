namespace Eduprompt.Domain.DTOs.Conversation;

public class ConversationDto
{
    public int ConversationId { get; set; }
    public int UserId { get; set; }
    public string? Title { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? LastActivity { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public int MessageCount { get; set; }
    public DateTime? LastMessageDate { get; set; }
}
