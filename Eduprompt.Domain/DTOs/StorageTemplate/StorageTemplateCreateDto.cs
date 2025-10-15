using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.StorageTemplate;

public class StorageTemplateCreateDto
{
    [Required(ErrorMessage = "Package ID is required")]
    public int PackageID { get; set; }
    
    [Required]
    [StringLength(200)]
    public string TemplateName { get; set; } = string.Empty;
    
    public bool IsFavorite { get; set; } = false;
} 
