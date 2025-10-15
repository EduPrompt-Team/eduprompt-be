namespace Eduprompt.Domain.DTOs.StorageTemplate;

public class StorageTemplateDto
{
    public int StorageID { get; set; }
    public int UserID { get; set; }
    public int PackageID { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public string? UserName { get; set; }
    public string? PackageName { get; set; }
    public string? PackageDescription { get; set; }
    public decimal? PackagePrice { get; set; }
} 
