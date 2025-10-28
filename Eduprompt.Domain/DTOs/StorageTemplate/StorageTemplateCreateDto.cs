using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.StorageTemplate;

public class StorageTemplateCreateDto
{
    [Required(ErrorMessage = "Package ID is required")]
    public int PackageId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string TemplateName { get; set; } = string.Empty;
    
    [StringLength(int.MaxValue)]
    public string? TemplateContent { get; set; }
    
    [StringLength(10)]
    public string? Grade { get; set; }
    
    [StringLength(50)]
    public string? Subject { get; set; }
    
    [StringLength(100)]
    public string? Chapter { get; set; }
    
    public bool? IsPublic { get; set; } = null; // default false on server
    
    public bool IsFavorite { get; set; } = false;
} 
