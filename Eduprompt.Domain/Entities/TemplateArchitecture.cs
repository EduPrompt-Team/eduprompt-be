using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

[Table("TemplateArchitectures")]
public partial class TemplateArchitecture
{
    [Key]
    [Column("ArchitectureID")]
    public int ArchitectureID { get; set; }

    [Required]
    [Column("StorageID")]
    public int StorageID { get; set; }

    [Required]
    [StringLength(100)]
    [Column("ArchitectureName")]
    public string ArchitectureName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Column("ArchitectureType")]
    public string ArchitectureType { get; set; } = "Sequential"; // e.g., 'Sequential', 'Conditional', 'Loop', 'Parallel'

    [Column("ConfigurationJson")]
    public string? ConfigurationJson { get; set; } // JSON configuration for the architecture

    // Navigation properties
    [ForeignKey("StorageID")]
    public virtual StorageTemplate StorageTemplate { get; set; } = null!;
}
