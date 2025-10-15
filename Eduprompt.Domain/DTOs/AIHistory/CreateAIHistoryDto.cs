using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.AIHistory;

public class CreateAIHistoryDto
{
    [Required]
    public int UserID { get; set; }

    public int? ConversationID { get; set; }

    public int? PromptInstanceID { get; set; }

    public string? UserMessage { get; set; }

    public string? AIResponse { get; set; }

    public int? ProcessingTimeMs { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Completed";
}
