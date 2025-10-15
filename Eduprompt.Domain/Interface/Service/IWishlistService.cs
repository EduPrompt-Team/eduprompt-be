using Eduprompt.Domain.DTOs.Wishlist;

namespace Eduprompt.Domain.Interface.Service;

public interface IWishlistService
{
    Task<IEnumerable<WishlistDto>> GetByUserIdAsync(int userId);
    Task<WishlistDto?> GetByIdAsync(int wishlistId);
    Task<WishlistDto> CreateAsync(int userId, WishlistCreateDto wishlistDto);
    Task<bool> DeleteAsync(int wishlistId);
    Task<bool> IsInWishlistAsync(int userId, int packageId);
} 