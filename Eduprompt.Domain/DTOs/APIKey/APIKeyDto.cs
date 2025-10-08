namespace Eduprompt.Domain.DTOs.APIKey;

public class APIKeyDto
{
    public int APIKeyID { get; set; }
    public int PackageID { get; set; }
    public string KeyName { get; set; } = string.Empty;
    public string? KeyValue { get; set; }
    public string? Provider { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Status { get; set; }
    public string? PackageName { get; set; }
}
