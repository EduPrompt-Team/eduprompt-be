using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class TemplateArchitecture
{
    [Key]
    public int ArchitectureID { get; set; }

    [Required]
    public int TemplateID { get; set; }

    [Required]
    [StringLength(100)]
    public string ArchitectureName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string ArchitectureType { get; set; } = string.Empty; // 'Sequential', 'Conditional', 'Loop', 'Parallel'

    public string? Configuration { get; set; } // JSON configuration

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey("TemplateID")]
    public virtual StorageTemplate StorageTemplate { get; set; } = null!;
}
