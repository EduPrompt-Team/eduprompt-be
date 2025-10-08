using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Post;

public class CreatePostDto
{
    [Required]
    public int UserID { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [StringLength(50)]
    public string? PostType { get; set; } = "General";

    [StringLength(500)]
    public string? Tags { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Published";
}
