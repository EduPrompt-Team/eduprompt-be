using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IWishlistRepository
{
    Task<Wishlist?> GetByIdAsync(int id);
    Task<IEnumerable<Wishlist>> GetByUserIdAsync(int userId);
    Task<Wishlist?> GetUserWishlistItemAsync(int userId, int templateId);
    Task<Wishlist> CreateAsync(Wishlist wishlist);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int userId, int templateId);
} 