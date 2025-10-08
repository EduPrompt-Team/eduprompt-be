using Eduprompt.Domain.Entities;

namespace Eduprompt.Domain.Interface.Repository;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int orderId);
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task<string> GenerateOrderNumberAsync();
} 