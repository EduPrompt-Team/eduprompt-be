using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class TemplateArchitecture
{
    [Key]
    public int ArchitectureID { get; set; }

    [Required]
    public int StorageID { get; set; }

    [Required]
    [StringLength(100)]
    public string ArchitectureName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string ArchitectureType { get; set; } = "Sequential"; // e.g., 'Sequential', 'Conditional', 'Loop', 'Parallel'

    public string? ConfigurationJson { get; set; } // JSON configuration for the architecture

    // Navigation properties
    [ForeignKey("StorageID")]
    public virtual StorageTemplate StorageTemplate { get; set; } = null!;
}
