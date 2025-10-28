namespace Eduprompt.Domain.DTOs.StorageTemplate;

public class StorageTemplateUpdateDto
{
    public string? TemplateName { get; set; }
    public string? TemplateContent { get; set; }
    public string? Grade { get; set; }
    public string? Subject { get; set; }
    public string? Chapter { get; set; }
    public bool? IsPublic { get; set; }
}


