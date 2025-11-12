using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.PromptInstance;

public class CompletePromptInstanceDto
{
    /// <summary>
    /// Output JSON data (conversation history, prompt result, etc.)
    /// </summary>
    public string? OutputJson { get; set; }

    /// <summary>
    /// Instance status (e.g., "Completed", "Failed")
    /// </summary>
    [StringLength(50)]
    public string? Status { get; set; } = "Completed";

    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    [Range(0, int.MaxValue)]
    public int? ProcessingTimeMs { get; set; }
}

