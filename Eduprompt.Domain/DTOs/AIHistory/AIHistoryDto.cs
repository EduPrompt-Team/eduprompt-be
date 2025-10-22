namespace Eduprompt.Domain.DTOs.Aihistory;

public class AihistoryDto
{
    public int HistoryID { get; set; }
    public int UserId { get; set; }
    public int? ConversationId { get; set; }
    public int? PromptInstanceId { get; set; }
    public string? UserMessage { get; set; }
    public string? Airesponse { get; set; }
    public DateTime ExecutedAt { get; set; }
    public int? ProcessingTimeMs { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public string? PromptInstanceName { get; set; }
}
