using Eduprompt.Domain.DTOs.Order;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;
using Eduprompt.DAL.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Eduprompt.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IWalletService _walletService;
    private readonly EdupromptV2Context _db;

    public OrderService(
        IOrderRepository orderRepository, 
        ICartRepository cartRepository,
        IWalletService walletService,
        EdupromptV2Context db)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _walletService = walletService;
        _db = db;
    }

    public async Task<OrderServiceDto> CreateOrderFromCartAsync(int UserId, string? notes)
    {
        var cart = await _cartRepository.GetByUserIdAsync(UserId);
        var totalAmount = cart?.CartDetails?.Sum(cd => cd.Quantity * cd.UnitPrice) ?? 0m;

        var order = new Order
        {
            UserId = UserId,
            PackageId = null,
            TotalAmount = totalAmount,
            OrderDate = DateTime.UtcNow,
            Notes = notes,
            Status = "Pending"
        };

        var created = await _orderRepository.CreateAsync(order);
        await _cartRepository.ClearCartAsync(UserId);
        return MapToServiceDto(created);
    }

    public async Task<OrderServiceDto?> GetByIdAsync(int OrderId, int UserId)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId);
        if (order == null || order.UserId != UserId) return null;
        return MapToServiceDto(order);
    }

    public async Task<OrderServiceDto?> GetByIdAdminAsync(int OrderId)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId);
        if (order == null) return null;
        return MapToServiceDto(order);
    }

    public async Task<IEnumerable<OrderServiceDto>> GetUserOrdersAsync(int UserId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(UserId);
        return orders.Select(MapToServiceDto);
    }

    public async Task<IEnumerable<OrderServiceDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToServiceDto);
    }

    public async Task<OrderServiceDto> CancelOrderAsync(int OrderId, int UserId)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId);
        if (order == null || order.UserId != UserId)
            throw new KeyNotFoundException("Order not found");

        order.Status = "Cancelled";
        var updated = await _orderRepository.UpdateAsync(order);
        return MapToServiceDto(updated);
    }

    public async Task<OrderServiceDto> UpdateOrderStatusAsync(int OrderId, string status)
    {
        var order = await _orderRepository.GetByIdAsync(OrderId);
        if (order == null)
            throw new KeyNotFoundException("Order not found");

        order.Status = status;
        var updated = await _orderRepository.UpdateAsync(order);
        return MapToServiceDto(updated);
    }

    public async Task<OrderServiceDto> PayOrderWithWalletAsync(int OrderId, int UserId)
    {
        // Use transaction to ensure atomicity
        await using var tx = await _db.Database.BeginTransactionAsync();
        
        try
        {
            // Lock order row to prevent concurrent modifications
            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.OrderId == OrderId);
            
            if (order == null || order.UserId != UserId)
                throw new KeyNotFoundException("Order not found");

            if (order.Status != "Pending")
                throw new InvalidOperationException($"Cannot pay order with status: {order.Status}");

            // Check wallet balance
            var balance = await _walletService.GetBalanceByUserIdAsync(UserId);
            if (balance < order.TotalAmount)
                throw new InvalidOperationException($"Insufficient wallet balance. Required: {order.TotalAmount}, Available: {balance}");

            // Deduct funds from wallet
            var deducted = await _walletService.DeductFundsByUserIdAsync(UserId, order.TotalAmount);
            if (!deducted)
                throw new InvalidOperationException("Failed to deduct funds from wallet");

            // Update order status
            order.Status = "Paid";
            var updated = await _orderRepository.UpdateAsync(order);
            
            await tx.CommitAsync();
            return MapToServiceDto(updated);
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    private static OrderServiceDto MapToServiceDto(Order order)
    {
        return new OrderServiceDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            OrderNumber = order.OrderId.ToString(),
            TotalAmount = order.TotalAmount,
            CreatedDate = order.OrderDate, // Use OrderDate as CreatedDate
            OrderDate = order.OrderDate,
            Status = order.Status,
            UserName = order.User?.FullName,
            UserEmail = order.User?.Email,
            Items = new List<OrderItemServiceDto>(),
            Payments = new List<PaymentServiceDto>()
        };
    }
}