using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("PackageCategories")]
public partial class PackageCategory
{
    [Key]
    [Column("CategoryID")]
    public int CategoryID { get; set; }

    [Required]
    [StringLength(100)]
    [Column("CategoryName")]
    public string CategoryName { get; set; } = string.Empty;

    [Column("Description")]
    public string? Description { get; set; }

    [Column("DisplayOrder")]
    public int DisplayOrder { get; set; } = 0;

    // Navigation properties
    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();
}
