using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Aihistory;

public class CreateAihistoryDto
{
    [Required]
    public int UserId { get; set; }

    public int? ConversationId { get; set; }

    public int? PromptInstanceId { get; set; }

    public string? UserMessage { get; set; }

    public string? Airesponse { get; set; }

    public int? ProcessingTimeMs { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Completed";
}
