using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class StorageTemplate
{
    [Key]
    public int StorageID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    public int PackageID { get; set; }

    [Required]
    [StringLength(200)]
    public string TemplateName { get; set; } = string.Empty;

    [Required]
    public bool IsFavorite { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("UserID")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PackageID")]
    public virtual Package Package { get; set; } = null!;

    public virtual ICollection<TemplateArchitecture> TemplateArchitectures { get; set; } = new List<TemplateArchitecture>();
}
