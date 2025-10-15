using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class Wishlist
{
    [Key]
    public int WishlistId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int PackageID { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PackageID")]
    public virtual Package Package { get; set; } = null!;
}