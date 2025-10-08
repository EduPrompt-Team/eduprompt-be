using Microsoft.AspNetCore.Http;

namespace Eduprompt.Domain.DTOs.Storage;

public class FileUploadDto
{
    public IFormFile File { get; set; } = default!;
    public string BucketName { get; set; } = "default";
    public string? FolderPath { get; set; }
}
