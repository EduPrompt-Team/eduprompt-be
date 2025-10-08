using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.StorageTemplate;

public class StorageTemplateCreateDto
{
    [Required(ErrorMessage = "Template ID is required")]
    public int TemplateId { get; set; }
} 
