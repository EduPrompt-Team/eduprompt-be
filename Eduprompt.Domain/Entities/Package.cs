using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("Packages")]
public partial class Package
{
    [Key]
    [Column("PackageID")]
    public int PackageID { get; set; }

    [Column("CategoryID")]
    public int? CategoryID { get; set; }

    [Required]
    [StringLength(100)]
    [Column("PackageName")]
    public string PackageName { get; set; } = string.Empty;

    [Column("Description")]
    public string? Description { get; set; }

    [Required]
    [Column("Price", TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    [Column("DurationDays")]
    public int? DurationDays { get; set; } // Duration in days

    [Required]
    [Column("IsActive")]
    public bool IsActive { get; set; } = true;

    [Required]
    [Column("CreatedDate")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("CategoryID")]
    public virtual PackageCategory? PackageCategory { get; set; }

    public virtual ICollection<PackageDetail> PackageDetails { get; set; } = new List<PackageDetail>();
    public virtual ICollection<APIKey> APIKeys { get; set; } = new List<APIKey>();
    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
