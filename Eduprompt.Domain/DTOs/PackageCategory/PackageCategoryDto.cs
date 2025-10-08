namespace Eduprompt.Domain.DTOs.PackageCategory;

public class PackageCategoryDto
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
    public int PackageCount { get; set; }
}
