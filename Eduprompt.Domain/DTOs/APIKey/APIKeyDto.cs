namespace Eduprompt.Domain.DTOs.APIKey;

public class APIKeyDto
{
    public int APIKeyID { get; set; }
    public int PackageID { get; set; }
    public string APIProvider { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;
    public int? UsageLimit { get; set; }
    public int CurrentUsage { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? PackageName { get; set; }
}
