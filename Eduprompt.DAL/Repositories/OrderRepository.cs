using Eduprompt.Domain.Entities;
using Eduprompt.DAL.DbContexts;
using Eduprompt.Domain.Interface.Repository;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.DAL.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly EdupromptV2Context _context;

    public OrderRepository(EdupromptV2Context context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int orderId)
    {
        return await _context.Orders
            // .Include(o => o.OrderDetails) // Removed - OrderDetails navigation property deleted
            //     .ThenInclude(od => od.Template) // Removed - OrderDetails navigation property deleted
            .Include(o => o.User)
            .Include(o => o.Payments) // Added - Include Payments
            .Include(o => o.Package) // Added - Include Package
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

    public async Task<IEnumerable<Order>> GetByUserIdAsync(int UserId)
    {
        return await _context.Orders
            // .Include(o => o.OrderDetails) // Removed - OrderDetails navigation property deleted
            //     .ThenInclude(od => od.Template) // Removed - OrderDetails navigation property deleted
            .Include(o => o.User)
            .Include(o => o.Payments) // Added - Include Payments
            .Include(o => o.Package) // Added - Include Package
            .Where(o => o.UserId == UserId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByUserIdAndPackageIdAsync(int userId, int packageId)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.Payments)
            .Include(o => o.Package)
            .Where(o => o.UserId == userId && o.PackageId == packageId)
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
        order.OrderDate = DateTime.UtcNow;
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

    public async Task<bool> OrderHasPackageInCartDetailsAsync(int orderId, int packageId)
    {
        // Check if there's a cart that was used to create this order
        // Note: This is a fallback check - after order is created, cart is cleared
        // So this might not work for existing orders
        // This is mainly for future orders or if cart data is preserved
        
        // For now, return false as we can't reliably check cart details after order creation
        // The main check should be in Orders.PackageId
        return false;
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
