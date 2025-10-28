namespace Eduprompt.Domain.DTOs.StorageTemplate;

public class StorageTemplateDto
{
    public int StorageId { get; set; }
    public int UserId { get; set; }
    public int PackageId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string? TemplateContent { get; set; }
    public string? Grade { get; set; }
    public string? Subject { get; set; }
    public string? Chapter { get; set; }
    public bool IsPublic { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public string? UserName { get; set; }
    public string? PackageName { get; set; }
    public string? PackageDescription { get; set; }
    public decimal? PackagePrice { get; set; }
    public string? TemplatePreviewUrl { get; set; }
} 
