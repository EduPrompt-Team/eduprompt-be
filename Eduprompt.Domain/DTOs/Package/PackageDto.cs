namespace Eduprompt.Domain.DTOs.Package;

public class PackageDto
{
    public int PackageID { get; set; }
    public int? CategoryID { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int? Duration { get; set; }
    public int? MaxUsage { get; set; }
    public string? Features { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
    public string? CategoryName { get; set; }
}
