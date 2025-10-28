namespace Eduprompt.Domain.Entities;

// Partial extension to map newly added DB columns
public partial class StorageTemplate
{
    public string? TemplateContent { get; set; }
    public string? Grade { get; set; }
    public string? Subject { get; set; }
    public string? Chapter { get; set; }
    public bool IsPublic { get; set; }
}


