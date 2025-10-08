using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Feedback;

public class CreateFeedbackDto
{
    [Required]
    public int PostID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Active";
}
