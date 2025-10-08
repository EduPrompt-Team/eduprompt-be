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

    [HttpPost("create-from-cart")]
    [Authorize]
    public async Task<IActionResult> CreateFromCart([FromQuery] string? notes)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var result = await _orderService.CreateOrderFromCartAsync(userId, notes);
        return Ok(result);
    }

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
        if (order == null) return NotFound();
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


