namespace Eduprompt.Domain.Interface.Service;

public interface IStorageTemplateService
{
    Task<IEnumerable<StorageTemplateServiceDto>> GetUserStorageAsync(int userId);
    Task<StorageTemplateServiceDto> AddToStorageAsync(int userId, StorageTemplateCreateServiceDto storageDto);
    Task<bool> RemoveFromStorageAsync(int id, int userId);
    Task<bool> IsInStorageAsync(int userId, int templateId);
}

public class StorageTemplateServiceDto
{
    public int StorageId { get; set; }
    public int UserId { get; set; }
    public int TemplateId { get; set; }
    public DateTime? UploadDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public string? TemplateName { get; set; }
    public string? TemplateDescription { get; set; }
    public decimal? TemplatePrice { get; set; }
    public string? TemplatePreviewUrl { get; set; }
}

public class StorageTemplateCreateServiceDto
{
    public int TemplateId { get; set; }
} 