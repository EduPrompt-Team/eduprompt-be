using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly EdupromptContext _context;

    public OrderRepository(EdupromptContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int orderId)
    {
        return await _context.Orders
            // .Include(o => o.OrderDetails) // Removed - OrderDetails navigation property deleted
            //     .ThenInclude(od => od.Template) // Removed - OrderDetails navigation property deleted
            .Include(o => o.User)
            // .Include(o => o.Payments) // Removed - Payments navigation property deleted
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _context.Orders
            // .Include(o => o.OrderDetails) // Removed - OrderDetails navigation property deleted
            //     .ThenInclude(od => od.Template) // Removed - OrderDetails navigation property deleted
            .Include(o => o.User)
            // .Include(o => o.Payments) // Removed - Payments navigation property deleted
            .FirstOrDefaultAsync(o => o.OrderId.ToString() == orderNumber);
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
    {
        return await _context.Orders
            // .Include(o => o.OrderDetails) // Removed - OrderDetails navigation property deleted
            //     .ThenInclude(od => od.Template) // Removed - OrderDetails navigation property deleted
            .Include(o => o.User)
            // .Include(o => o.Payments) // Removed - Payments navigation property deleted
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            // .Include(o => o.OrderDetails) // Removed - OrderDetails navigation property deleted
            //     .ThenInclude(od => od.Template) // Removed - OrderDetails navigation property deleted
            .Include(o => o.User)
            // .Include(o => o.Payments) // Removed - Payments navigation property deleted
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order> CreateAsync(Order order)
    {
        order.OrderDate = DateTime.Now;
        order.OrderDate = DateTime.Now;
        order.Status = order.Status ?? "Pending";

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(order.OrderId) ?? order;
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(order.OrderId) ?? order;
    }

    public async Task<string> GenerateOrderNumberAsync()
    {
        var now = DateTime.Now;
        var prefix = $"ORD{now:yyyyMMdd}";
        
        var lastOrder = await _context.Orders
            .Where(o => o.OrderId.ToString().StartsWith(prefix))
            .OrderByDescending(o => o.OrderId)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastOrder != null)
        {
            var lastSequence = lastOrder.OrderId.ToString().Substring(prefix.Length);
            if (int.TryParse(lastSequence, out int num))
            {
                sequence = num + 1;
            }
        }

        return $"{prefix}{sequence:D4}";
    }
} 