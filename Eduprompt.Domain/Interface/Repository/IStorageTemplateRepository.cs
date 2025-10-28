using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IStorageTemplateRepository
{
    Task<StorageTemplate?> GetByIdAsync(int id);
    Task<IEnumerable<StorageTemplate>> GetByUserIdAsync(int userId);
    Task<StorageTemplate?> GetUserStorageItemAsync(int userId, int templateId);
    Task<StorageTemplate> CreateAsync(StorageTemplate storage);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int userId, int templateId);
    Task<IEnumerable<StorageTemplate>> GetPublicAsync(int? packageId, string? grade, string? subject, string? chapter);
    Task<StorageTemplate?> UpdateAsync(StorageTemplate entity);
    Task<bool> SetPublishAsync(int id, bool isPublic);
} 