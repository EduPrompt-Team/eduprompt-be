namespace Eduprompt.Domain.DTOs.Conversation;

public class ConversationDto
{
    public int ConversationID { get; set; }
    public int UserID { get; set; }
    public string Title { get; set; } = string.Empty;
    // public string? Description { get; set; } // Removed - Conversation entity doesn't have Description property
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public int MessageCount { get; set; }
    public DateTime? LastMessageDate { get; set; }
}
