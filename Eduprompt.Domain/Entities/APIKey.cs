using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eduprompt.Domain.Entities;

public partial class APIKey
{
    [Key]
    public int APIKeyID { get; set; }

    [Required]
    public int PackageID { get; set; }

    [Required]
    [StringLength(100)]
    public string KeyName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string KeyValue { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Provider { get; set; } = string.Empty; // 'OpenAI', 'Anthropic', 'Google', 'Custom'

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiryDate { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Active";

    // Navigation properties
    [ForeignKey("PackageID")]
    public virtual Package Package { get; set; } = null!;
}
