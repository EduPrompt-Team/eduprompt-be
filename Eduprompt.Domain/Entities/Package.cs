using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class Package
{
    [Key]
    public int PackageID { get; set; }

    public int? CategoryID { get; set; }

    [Required]
    [StringLength(100)]
    public string PackageName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int? Duration { get; set; } // Duration in days

    public int? MaxUsage { get; set; } // Maximum usage count

    public string? Features { get; set; } // JSON string of features

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey("CategoryID")]
    public virtual PackageCategory? PackageCategory { get; set; }

    public virtual ICollection<PackageDetail> PackageDetails { get; set; } = new List<PackageDetail>();
    public virtual ICollection<APIKey> APIKeys { get; set; } = new List<APIKey>();
    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    // public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>(); // Removed - OrderDetail entity deleted
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
