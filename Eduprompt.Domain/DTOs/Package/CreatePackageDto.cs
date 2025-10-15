using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Package;

public class CreatePackageDto
{
    public int? CategoryID { get; set; }

    [Required]
    [StringLength(100)]
    public string PackageName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative")]
    public decimal Price { get; set; }

    public int? DurationDays { get; set; }

    public bool IsActive { get; set; } = true;
}
