namespace Eduprompt.Domain.DTOs.StorageTemplate;

public class StorageTemplateDto
{
    public int StorageId { get; set; }
    public int UserId { get; set; }
    public int TemplateId { get; set; }
    public DateTime? UploadDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
    
    // Navigation properties
    public string? UserName { get; set; }
    public string? TemplateName { get; set; }
    public string? TemplateDescription { get; set; }
    public decimal? TemplatePrice { get; set; }
    public string? TemplatePreviewUrl { get; set; }
} 
