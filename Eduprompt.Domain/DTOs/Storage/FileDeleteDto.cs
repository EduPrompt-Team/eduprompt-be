namespace Eduprompt.Domain.DTOs.Storage;

public class FileDeleteDto
{
    public string BucketName { get; set; } = "default";
    public string FilePath { get; set; } = string.Empty;
}
