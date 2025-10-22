namespace Eduprompt.Domain.DTOs.Apikey;

public class ApikeyDto
{
    public int ApikeyId { get; set; }
    public int PackageId { get; set; }
    public string Apiprovider { get; set; } = string.Empty;
    public string KeyHash { get; set; } = string.Empty;
    public int? UsageLimit { get; set; }
    public int CurrentUsage { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? PackageName { get; set; }
}
