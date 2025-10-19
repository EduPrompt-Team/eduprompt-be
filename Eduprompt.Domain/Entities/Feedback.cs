using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("Feedbacks")]
public partial class Feedback
{
    [Key]
    [Column("FeedbackID")]
    public int FeedbackID { get; set; }

    [Required]
    [Column("PostID")]
    public int PostID { get; set; }

    [Required]
    [Column("UserID")]
    public int UserID { get; set; }

    [Column("PackageID")]
    public int? PackageID { get; set; }

    [Required]
    [Range(1, 5)]
    [Column("Rating")]
    public int Rating { get; set; } // 1-5 stars

    [Column("Comment")]
    public string? Comment { get; set; }

    [Required]
    [Column("CreatedDate")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Required]
    [Column("IsVerified")]
    public bool IsVerified { get; set; } = false;

    [StringLength(50)]
    [Column("Status")]
    public string? Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey("PostID")]
    public virtual Post Post { get; set; } = null!;

    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PackageID")]
    public virtual Package? Package { get; set; }
}
