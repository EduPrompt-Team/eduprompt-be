using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class Feedback
{
    [Key]
    public int FeedbackID { get; set; }

    [Required]
    public int PostID { get; set; }

    [Required]
    public int UserID { get; set; }

    public int? PackageID { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; } // 1-5 stars

    public string? Comment { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsVerified { get; set; } = false;

    [StringLength(50)]
    public string? Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey("PostID")]
    public virtual Post Post { get; set; } = null!;

    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PackageID")]
    public virtual Package? Package { get; set; }
}
