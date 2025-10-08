using AutoMapper;
using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;

namespace Eduprompt.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IStorageTemplateRepository _storageRepository;
    private readonly IMapper _mapper;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IStorageTemplateRepository storageRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _storageRepository = storageRepository;
        _mapper = mapper;
    }

    public async Task<OrderServiceDto> CreateOrderFromCartAsync(int userId, string? notes)
    {
        // Get user's cart
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        
        if (cart == null || cart.CartDetails == null || !cart.CartDetails.Any())
        {
            throw new InvalidOperationException("Cart is empty");
        }

        // Generate order number
        var orderNumber = await _orderRepository.GenerateOrderNumberAsync();

        // Calculate total
        decimal totalAmount = cart.CartDetails.Sum(cd => cd.SubTotal ?? 0);

        // Create order
        var order = new Order
        {
            UserId = userId,
            OrderNumber = orderNumber,
            TotalAmount = totalAmount,
            Status = "Pending"
        };

        var createdOrder = await _orderRepository.CreateAsync(order);

        // Create order details from cart items
        // foreach (var cartItem in cart.CartDetails)
        // {
        //     var orderDetail = new OrderDetail
        //     {
        //         OrderId = createdOrder.OrderId,
        //         TemplateId = cartItem.TemplateId,
        //         Quantity = cartItem.Quantity,
        //         Price = cartItem.UnitPrice,
        //         Status = "Active"
        //     };

        //     // Note: OrderDetails will be added through EF navigation
        //     createdOrder.OrderDetails.Add(orderDetail);
        // } // Removed - OrderDetail entity deleted

        await _orderRepository.UpdateAsync(createdOrder);

        // Clear cart after successful order
        await _cartRepository.ClearCartAsync(userId);

        return _mapper.Map<OrderServiceDto>(await _orderRepository.GetByIdAsync(createdOrder.OrderId));
    }

    public async Task<OrderServiceDto?> GetByIdAsync(int orderId, int userId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        
        if (order == null)
            return null;

        // Only return if user owns the order
        if (order.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only view your own orders");
        }

        return _mapper.Map<OrderServiceDto>(order);
    }

    public async Task<IEnumerable<OrderServiceDto>> GetUserOrdersAsync(int userId)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<OrderServiceDto>>(orders);
    }

    public async Task<IEnumerable<OrderServiceDto>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<OrderServiceDto>>(orders);
    }

    public async Task<OrderServiceDto> CancelOrderAsync(int orderId, int userId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found");
        }

        // Verify ownership
        if (order.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only cancel your own orders");
        }

        // Can only cancel pending orders
        if (order.Status != "Pending")
        {
            throw new InvalidOperationException($"Cannot cancel order with status: {order.Status}");
        }

        order.Status = "Cancelled";
        var updatedOrder = await _orderRepository.UpdateAsync(order);

        return _mapper.Map<OrderServiceDto>(updatedOrder);
    }

    public async Task<OrderServiceDto> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        
        if (order == null)
        {
            throw new KeyNotFoundException($"Order with ID {orderId} not found");
        }

        order.Status = status;

        // If order is completed, add templates to user's storage
        // if (status == "Completed" && order.OrderDetails != null)
        // {
        //     foreach (var item in order.OrderDetails)
        //     {
        //         // Check if not already in storage
        //         if (!await _storageRepository.ExistsAsync(order.UserId, item.TemplateId))
        //         {
        //             await _storageRepository.CreateAsync(new StorageTemplate
        //             {
        //                 UserId = order.UserId,
        //                 TemplateId = item.TemplateId
        //             });
        //         }
        //     }
        // } // Removed - OrderDetails navigation property deleted

        var updatedOrder = await _orderRepository.UpdateAsync(order);
        return _mapper.Map<OrderServiceDto>(updatedOrder);
    }
} 
