using Eduprompt.Domain.DTOs.Wishlist;

namespace Eduprompt.Domain.Interface.Service;

public interface IWishlistService
{
    Task<IEnumerable<WishlistDto>> GetByUserIdAsync(int userId);
    Task<WishlistDto?> GetByIdAsync(int wishlistId);
    Task<WishlistDto> CreateAsync(int userId, WishlistCreateDto wishlistDto);
    Task<bool> DeleteAsync(int wishlistId);
    Task<bool> DeleteByStorageIdAsync(int userId, int storageId);  // New - delete by StorageId
    Task<bool> IsInWishlistAsync(int userId, int packageId);  // Legacy - by PackageId
    Task<bool> IsInWishlistByStorageIdAsync(int userId, int storageId);  // New - by StorageId
} 