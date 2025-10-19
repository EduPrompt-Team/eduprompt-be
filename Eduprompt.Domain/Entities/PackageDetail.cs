using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("PackageDetails")]
public partial class PackageDetail
{
    [Key]
    [Column("DetailID")]
    public int DetailID { get; set; }

    [Required]
    [Column("PackageID")]
    public int PackageID { get; set; }

    [Required]
    [StringLength(100)]
    [Column("FeatureName")]
    public string FeatureName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    [Column("FeatureValue")]
    public string FeatureValue { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Column("FeatureType")]
    public string FeatureType { get; set; } = string.Empty; // 'Text', 'Number', 'Boolean', 'List'

    // Navigation properties
    [ForeignKey("PackageID")]
    public virtual Package Package { get; set; } = null!;
}
