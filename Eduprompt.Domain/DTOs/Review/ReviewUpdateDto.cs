using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Review;

public class ReviewUpdateDto
{
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(5000, ErrorMessage = "Comment cannot exceed 5000 characters")]
    public string? Comment { get; set; }
}

