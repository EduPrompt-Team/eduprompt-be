namespace Eduprompt.Domain.DTOs.PackageDetail;

public class PackageDetailDto
{
    public int DetailId { get; set; }
    public int PackageId { get; set; }
    public string FeatureName { get; set; } = string.Empty;
    public string? FeatureDescription { get; set; }
    public bool IsIncluded { get; set; }
    public string? Unit { get; set; }
    public string? PackageName { get; set; }
}
