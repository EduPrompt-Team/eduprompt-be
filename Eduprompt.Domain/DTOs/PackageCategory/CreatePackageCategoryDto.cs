using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.PackageCategory;

public class CreatePackageCategoryDto
{
    [Required]
    [StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Active";
}
