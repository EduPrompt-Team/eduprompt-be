namespace Eduprompt.Domain.DTOs.Storage;

public class FileUploadResponseDto
{
    public string FileName { get; set; } = string.Empty;
    public string PublicUrl { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
