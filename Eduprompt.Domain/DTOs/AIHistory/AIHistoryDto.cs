namespace Eduprompt.Domain.DTOs.AIHistory;

public class AIHistoryDto
{
    public int HistoryID { get; set; }
    public int UserID { get; set; }
    public int? PromptInstanceID { get; set; }
    public string InputText { get; set; } = string.Empty;
    public string? OutputText { get; set; }
    public string? ModelUsed { get; set; }
    public int? TokensUsed { get; set; }
    public decimal? Cost { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public string? InstanceName { get; set; }
}
