using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.APIKey;

public class CreateAPIKeyDto
{
    [Required]
    public int PackageID { get; set; }

    [Required]
    [StringLength(100)]
    public string KeyName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? KeyValue { get; set; }

    [StringLength(50)]
    public string? Provider { get; set; }

    public DateTime? ExpiryDate { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Active";
}
