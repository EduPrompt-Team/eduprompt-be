namespace Eduprompt.Domain.DTOs.Storage;

public class FileListDto
{
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public string UpdatedAt { get; set; } = string.Empty;
    public string LastAccessedAt { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public long Size { get; set; }
    public string MimeType { get; set; } = string.Empty;
}
