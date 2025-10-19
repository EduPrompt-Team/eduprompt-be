using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "14. Order")]
[Produces("application/json")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Create order from user's cart
    /// </summary>
    /// <param name="notes">Optional order notes</param>
    /// <returns>Created order details</returns>
    /// <response code="200">Order created successfully</response>
    /// <response code="400">Invalid cart or order data</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost("create-from-cart")]
    [Authorize]
    public async Task<IActionResult> CreateFromCart([FromQuery] string? notes)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _orderService.CreateOrderFromCartAsync(userId, notes);
        return Ok(result);
    }

    /// <summary>
    /// Get all orders (Admin only)
    /// </summary>
    /// <returns>List of all orders</returns>
    /// <response code="200">Orders retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not authorized (Admin role required)</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var orders = await _orderService.GetUserOrdersAsync(userId);
        return Ok(orders);
    }

    [HttpGet("{orderId}")]
    [Authorize]
    public async Task<IActionResult> GetById(int orderId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var order = await _orderService.GetByIdAsync(orderId, userId);
        if (order == null) 
            return NotFound(new { message = "Order not found or you don't have permission to view this order" });
        return Ok(order);
    }

    /// <summary>
    /// Get order by ID (Admin only - can view any order)
    /// </summary>
    [HttpGet("admin/{orderId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByIdAdmin(int orderId)
    {
        var order = await _orderService.GetByIdAdminAsync(orderId);
        if (order == null) 
            return NotFound(new { message = "Order not found" });
        return Ok(order);
    }

    [HttpPost("{orderId}/cancel")]
    [Authorize]
    public async Task<IActionResult> Cancel(int orderId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var order = await _orderService.CancelOrderAsync(orderId, userId);
        return Ok(order);
    }

    [HttpPatch("{orderId}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int orderId, [FromQuery] string status)
    {
        var updated = await _orderService.UpdateOrderStatusAsync(orderId, status);
        return Ok(updated);
    }
}


