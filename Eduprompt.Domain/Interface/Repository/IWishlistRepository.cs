using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IWishlistRepository
{
    Task<Wishlist?> GetByIdAsync(int id);
    Task<IEnumerable<Wishlist>> GetByUserIdAsync(int userId);
    Task<Wishlist?> GetUserWishlistItemAsync(int userId, int templateId);  // Legacy - by PackageId
    Task<Wishlist?> GetUserWishlistItemByStorageIdAsync(int userId, int storageId);  // New - by StorageId
    Task<Wishlist> CreateAsync(Wishlist wishlist);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByStorageIdAsync(int userId, int storageId);  // New - delete by StorageId
    Task<bool> ExistsAsync(int userId, int templateId);  // Legacy - by PackageId
    Task<bool> ExistsByStorageIdAsync(int userId, int storageId);  // New - by StorageId
} 