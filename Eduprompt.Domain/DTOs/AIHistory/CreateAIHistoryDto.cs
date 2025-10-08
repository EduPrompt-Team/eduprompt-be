using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.AIHistory;

public class CreateAIHistoryDto
{
    [Required]
    public int UserID { get; set; }

    public int? PromptInstanceID { get; set; }

    [Required]
    public string InputText { get; set; } = string.Empty;

    public string? OutputText { get; set; }

    [StringLength(100)]
    public string? ModelUsed { get; set; }

    public int? TokensUsed { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Cost must be non-negative")]
    public decimal? Cost { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Completed";
}
