using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("Wishlists")]
public partial class Wishlist
{
    [Key]
    [Column("WishlistID")]
    public int WishlistId { get; set; }

    [Required]
    [Column("UserId")]
    public int UserId { get; set; }

    [Required]
    [Column("PackageID")]
    public int PackageID { get; set; }

    [Column("AddedAt")]
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    [Column("Notes")]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PackageID")]
    public virtual Package Package { get; set; } = null!;
}