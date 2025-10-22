using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.PackageDetail;

public class CreatePackageDetailDto
{
    [Required]
    public int PackageId { get; set; }

    [Required]
    [StringLength(100)]
    public string FeatureName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? FeatureDescription { get; set; }

    public bool IsIncluded { get; set; } = true;

    public int? Limit { get; set; }

    [StringLength(20)]
    public string? Unit { get; set; }
}
