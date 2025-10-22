using Eduprompt.Domain.DTOs.Cart;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eduprompt.API.Controllers;

/// <summary>
/// Shopping cart management for authenticated users
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[ApiExplorerSettings(GroupName = "18. Shopping Cart")]
[Produces("application/json")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        // Temporarily use default UserId for testing since authorize is disabled
        var UserId = 1; // Default user ID for testing
        // var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var cart = await _cartService.GetUserCartAsync(UserId);
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemDto itemDto)
    {
        // Temporarily use default UserId for testing since authorize is disabled
        var UserId = 1; // Default user ID for testing
        // var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var cart = await _cartService.AddItemAsync(UserId, itemDto);
        return Ok(cart);
    }

    [HttpPut("items/{cartDetailId}")]
    public async Task<IActionResult> UpdateItem(int cartDetailId, [FromBody] UpdateCartItemDto itemDto)
    {
        // Temporarily use default UserId for testing since authorize is disabled
        var UserId = 1; // Default user ID for testing
        // var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var cart = await _cartService.UpdateItemQuantityAsync(UserId, cartDetailId, itemDto.Quantity);
        return Ok(cart);
    }

    [HttpDelete("items/{cartDetailId}")]
    public async Task<IActionResult> RemoveItem(int cartDetailId)
    {
        // Temporarily use default UserId for testing since authorize is disabled
        var UserId = 1; // Default user ID for testing
        // var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _cartService.RemoveItemAsync(UserId, cartDetailId);
        if (!result) return NotFound(new { message = $"Cart item with ID {cartDetailId} not found" });
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        // Temporarily use default UserId for testing since authorize is disabled
        var UserId = 1; // Default user ID for testing
        // var UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _cartService.ClearCartAsync(UserId);
        if (!result) return NotFound(new { message = "Cart not found" });
        return NoContent();
    }
} 
