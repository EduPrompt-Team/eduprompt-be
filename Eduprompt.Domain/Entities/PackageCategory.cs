using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class PackageCategory
{
    [Key]
    public int CategoryID { get; set; }

    public int? ParentCategoryID { get; set; }

    [Required]
    [StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey("ParentCategoryID")]
    public virtual PackageCategory? ParentCategory { get; set; }

    public virtual ICollection<PackageCategory> SubCategories { get; set; } = new List<PackageCategory>();
    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();
}
