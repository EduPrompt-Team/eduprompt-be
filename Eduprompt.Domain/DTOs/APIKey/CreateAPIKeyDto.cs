using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Apikey;

public class CreateApikeyDto
{
    [Required]
    public int PackageId { get; set; }

    [Required]
    [StringLength(100)]
    public string Apiprovider { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string KeyHash { get; set; } = string.Empty;

    public int? UsageLimit { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
