using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class PackageDetail
{
    [Key]
    public int DetailID { get; set; }

    [Required]
    public int PackageID { get; set; }

    [Required]
    [StringLength(50)]
    public string DetailType { get; set; } = string.Empty; // 'Feature', 'Benefit', 'Requirement'

    [Required]
    public string DetailContent { get; set; } = string.Empty;

    public int OrderIndex { get; set; } = 0;

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [StringLength(50)]
    public string? Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey("PackageID")]
    public virtual Package Package { get; set; } = null!;
}
