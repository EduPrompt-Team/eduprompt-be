using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class PackageCategory
{
    [Key]
    public int CategoryID { get; set; }

    [Required]
    [StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int DisplayOrder { get; set; } = 0;

    // Navigation properties
    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();
}
