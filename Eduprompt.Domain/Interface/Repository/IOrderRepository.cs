using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int orderId);
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Order>> GetByUserIdAndPackageIdAsync(int userId, int packageId);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    
    // Check if order has packageId in CartDetails (for orders from cart)
    Task<bool> OrderHasPackageInCartDetailsAsync(int orderId, int packageId);
} 