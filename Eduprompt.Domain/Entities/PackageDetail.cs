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
    [StringLength(100)]
    public string FeatureName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string FeatureValue { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string FeatureType { get; set; } = string.Empty; // 'Text', 'Number', 'Boolean', 'List'

    // Navigation properties
    [ForeignKey("PackageID")]
    public virtual Package Package { get; set; } = null!;
}
