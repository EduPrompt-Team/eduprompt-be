using Eduprompt.Domain.DTOs.Order;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;

    public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
    }

    public async Task<OrderServiceDto> CreateOrderFromCartAsync(int userId, string? notes)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        var totalAmount = cart?.CartDetails?.Sum(cd => cd.Quantity * cd.UnitPrice) ?? 0m;

        var order = new Order
        {
            UserId = userId,
            PackageID = null,
            TotalAmount = totalAmount,
            OrderDate = DateTime.UtcNow,
            Notes = notes,
            Status = "Pending"
        };

        var created = await _orderRepository.CreateAsync(order);
        await _cartRepository.ClearCartAsync(userId);
        return MapToServiceDto(created);
    }

    public async Task<OrderServiceDto?> GetByIdAsync(int orderId, int userId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.UserId != userId) return null;
        return MapToServiceDto(order);
    }

    public async Task<IEnumerable<OrderServiceDto>> GetUserOrdersAsync(int userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        return orders.Select(MapToServiceDto);
    }

    public async Task<IEnumerable<OrderServiceDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return orders.Select(MapToServiceDto);
    }

    public async Task<OrderServiceDto> CancelOrderAsync(int orderId, int userId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null || order.UserId != userId)
            throw new KeyNotFoundException("Order not found");

        order.Status = "Cancelled";
        var updated = await _orderRepository.UpdateAsync(order);
        return MapToServiceDto(updated);
    }

    public async Task<OrderServiceDto> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new KeyNotFoundException("Order not found");

        order.Status = status;
        var updated = await _orderRepository.UpdateAsync(order);
        return MapToServiceDto(updated);
    }

    private static OrderServiceDto MapToServiceDto(Order order)
    {
        return new OrderServiceDto
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            OrderNumber = order.OrderId.ToString(),
            TotalAmount = order.TotalAmount,
            CreatedDate = null,
            OrderDate = order.OrderDate,
            Status = order.Status,
            UserName = order.User?.FullName,
            UserEmail = order.User?.Email,
            Items = new List<OrderItemServiceDto>(),
            Payments = new List<PaymentServiceDto>()
        };
    }
}