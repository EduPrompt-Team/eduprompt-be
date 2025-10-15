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
    public string APIProvider { get; set; } = string.Empty; // 'OpenAI', 'Anthropic', 'Google', 'Gemini'

    [Required]
    [StringLength(500)]
    public string KeyHash { get; set; } = string.Empty;

    public int? UsageLimit { get; set; }

    [Required]
    public int CurrentUsage { get; set; } = 0;

    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    [ForeignKey("PackageID")]
    public virtual Package Package { get; set; } = null!;
}
