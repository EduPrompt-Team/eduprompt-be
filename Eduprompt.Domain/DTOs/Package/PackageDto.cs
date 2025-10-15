namespace Eduprompt.Domain.DTOs.Package;

public class PackageDto
{
    public int PackageID { get; set; }
    public int? CategoryID { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int? DurationDays { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CategoryName { get; set; }
}
