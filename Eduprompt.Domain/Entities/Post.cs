using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class Post
{
    [Key]
    public int PostID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [StringLength(50)]
    public string? PostType { get; set; } = "General"; // 'General', 'Question', 'Share', 'Review'

    [StringLength(500)]
    public string? Tags { get; set; }

    public int ViewCount { get; set; } = 0;

    public int LikeCount { get; set; } = 0;

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Published";

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
