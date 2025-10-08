using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Package;

public class UpdatePackageDto
{
    public int? CategoryID { get; set; }

    [StringLength(100)]
    public string? PackageName { get; set; }

    public string? Description { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative")]
    public decimal? Price { get; set; }

    public int? Duration { get; set; }

    public int? MaxUsage { get; set; }

    public string? Features { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }
}
