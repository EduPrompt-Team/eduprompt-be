namespace Eduprompt.Domain.DTOs.AIHistory;

public class AIHistoryDto
{
    public int HistoryID { get; set; }
    public int UserID { get; set; }
    public int? ConversationID { get; set; }
    public int? PromptInstanceID { get; set; }
    public string? UserMessage { get; set; }
    public string? AIResponse { get; set; }
    public DateTime ExecutedAt { get; set; }
    public int? ProcessingTimeMs { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public string? InstanceName { get; set; }
}
