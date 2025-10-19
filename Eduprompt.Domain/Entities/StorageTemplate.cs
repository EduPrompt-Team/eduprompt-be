using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("StorageTemplates")]
public partial class StorageTemplate
{
    [Key]
    [Column("StorageID")]
    public int StorageID { get; set; }

    [Required]
    [Column("UserID")]
    public int UserID { get; set; }

    [Required]
    [Column("PackageID")]
    public int PackageID { get; set; }

    [Required]
    [StringLength(200)]
    [Column("TemplateName")]
    public string TemplateName { get; set; } = string.Empty;

    [Required]
    [Column("IsFavorite")]
    public bool IsFavorite { get; set; } = false;

    [Required]
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PackageID")]
    public virtual Package Package { get; set; } = null!;

    public virtual ICollection<TemplateArchitecture> TemplateArchitectures { get; set; } = new List<TemplateArchitecture>();
}
