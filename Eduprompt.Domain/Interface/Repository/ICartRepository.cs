using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(int userId);
    Task<Cart?> GetByIdAsync(int cartId);
    Task<Cart> CreateAsync(Cart cart);
    Task<Cart> UpdateAsync(Cart cart);
    Task<bool> ClearCartAsync(int userId);
    
    // Cart Items
    Task<CartDetail?> GetCartItemAsync(int cartDetailId);
    Task<CartDetail?> GetCartItemByPackageAsync(int cartId, int packageId);
    Task<CartDetail> AddItemAsync(CartDetail cartDetail);
    Task<CartDetail> UpdateItemAsync(CartDetail cartDetail);
    Task<bool> RemoveItemAsync(int cartDetailId);
} 