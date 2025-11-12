using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Review;

public class ReviewCreateDto
{
    [Required(ErrorMessage = "Storage ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Storage ID must be greater than 0")]
    public int StorageId { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(5000, ErrorMessage = "Comment cannot exceed 5000 characters")]
    public string? Comment { get; set; }

    // Optional package reference for backward compatibility with package-based reviews
    [Range(1, int.MaxValue, ErrorMessage = "Package ID must be greater than 0")]
    public int? PackageId { get; set; }
}

