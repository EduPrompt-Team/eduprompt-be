using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.APIKey;

public class CreateAPIKeyDto
{
    [Required]
    public int PackageID { get; set; }

    [Required]
    [StringLength(100)]
    public string APIProvider { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string KeyHash { get; set; } = string.Empty;

    public int? UsageLimit { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
