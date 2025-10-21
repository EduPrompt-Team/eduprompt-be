namespace Eduprompt.Domain.DTOs.PackageCategory;

public class PackageCategoryDto
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public int PackageCount { get; set; }
}
