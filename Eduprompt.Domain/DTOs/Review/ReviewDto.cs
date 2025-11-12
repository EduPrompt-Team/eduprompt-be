using System;

namespace Eduprompt.Domain.DTOs.Review;

public class ReviewDto
{
    public int ReviewId { get; set; }
    public int StorageId { get; set; }
    public int UserId { get; set; }
    public int? PackageId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ReviewUserDto? User { get; set; }
}

public class ReviewUserDto
{
    public int UserId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? ProfileUrl { get; set; }
}

