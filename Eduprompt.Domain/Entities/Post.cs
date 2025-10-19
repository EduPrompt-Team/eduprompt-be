using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("Posts")]
public partial class Post
{
    [Key]
    [Column("PostID")]
    public int PostID { get; set; }

    [Required]
    [Column("UserID")]
    public int UserID { get; set; }

    [Column("PackageID")]
    public int? PackageID { get; set; }

    [Required]
    [StringLength(200)]
    [Column("Title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Column("Content")]
    public string Content { get; set; } = string.Empty;

    [Column("ViewCount")]
    public int ViewCount { get; set; } = 0;

    [Required]
    [Column("PublishedAt")]
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    [Column("Status")]
    public string? Status { get; set; } = "Published";

    [StringLength(50)]
    [Column("PostType")]
    public string? PostType { get; set; } = "General";

    [StringLength(500)]
    [Column("Tags")]
    public string? Tags { get; set; }

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PackageID")]
    public virtual Package? Package { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
