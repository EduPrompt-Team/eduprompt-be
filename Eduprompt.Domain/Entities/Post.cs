using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class Post
{
    [Key]
    public int PostID { get; set; }

    [Required]
    public int UserID { get; set; }

    public int? PackageID { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    public int ViewCount { get; set; } = 0;

    [Required]
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    public string? Status { get; set; } = "Published";

    [StringLength(50)]
    public string? PostType { get; set; } = "General";

    [StringLength(500)]
    public string? Tags { get; set; }

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PackageID")]
    public virtual Package? Package { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
