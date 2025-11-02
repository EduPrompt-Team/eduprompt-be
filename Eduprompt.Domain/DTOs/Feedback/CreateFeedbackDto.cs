using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Feedback;

public class CreateFeedbackDto
{
    public int? PostId { get; set; }

    public int? StorageId { get; set; }

    public int? UserId { get; set; } // Optional - will be set from JWT token

    public int? PackageId { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    public bool IsVerified { get; set; } = false;

    [StringLength(50)]
    public string? Status { get; set; } = "Active";
}
