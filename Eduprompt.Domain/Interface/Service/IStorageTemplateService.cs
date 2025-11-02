namespace Eduprompt.Domain.Interface.Service;

public interface IStorageTemplateService
{
    Task<IEnumerable<StorageTemplateServiceDto>> GetUserStorageAsync(int userId);
    Task<StorageTemplateServiceDto> AddToStorageAsync(int userId, StorageTemplateCreateServiceDto storageDto);
    Task<bool> RemoveFromStorageAsync(int id, int userId);
    Task<bool> IsInStorageAsync(int userId, int templateId);
    Task<IEnumerable<StorageTemplateServiceDto>> GetPublicAsync(int? packageId, string? grade, string? subject, string? chapter);
    Task<StorageTemplateServiceDto?> UpdateAsync(int id, int currentUserId, StorageTemplateUpdateServiceDto updateDto, bool currentUserIsAdmin);
    Task<bool> PublishAsync(int id, bool isPublish, int currentUserId, bool currentUserIsAdmin);
}

public class StorageTemplateServiceDto
{
    public int StorageId { get; set; }
    public int UserId { get; set; }
    public int TemplateId { get; set; }
    public string? TemplateName { get; set; }
    public string? TemplateContent { get; set; }
    public string? Grade { get; set; }
    public string? Subject { get; set; }
    public string? Chapter { get; set; }
    public bool IsPublic { get; set; }
    public DateTime? UploadDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public string? TemplateDescription { get; set; }
    public decimal? TemplatePrice { get; set; }
    public string? TemplatePreviewUrl { get; set; }
}

public class StorageTemplateCreateServiceDto
{
    public int TemplateId { get; set; }
    public string? TemplateName { get; set; }
    public string? TemplateContent { get; set; }
    public string? Grade { get; set; }
    public string? Subject { get; set; }
    public string? Chapter { get; set; }
    public bool? IsPublic { get; set; }
} 

public class StorageTemplateUpdateServiceDto
{
    public string? TemplateName { get; set; }
    public string? TemplateContent { get; set; }
    public string? Grade { get; set; }
    public string? Subject { get; set; }
    public string? Chapter { get; set; }
    public bool? IsPublic { get; set; }
}