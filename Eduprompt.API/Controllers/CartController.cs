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
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var cart = await _cartService.GetUserCartAsync(userId);
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemDto itemDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var cart = await _cartService.AddItemAsync(userId, itemDto);
        return Ok(cart);
    }

    [HttpPut("items/{cartDetailId}")]
    public async Task<IActionResult> UpdateItem(int cartDetailId, [FromBody] UpdateCartItemDto itemDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var cart = await _cartService.UpdateItemQuantityAsync(userId, cartDetailId, itemDto.Quantity);
        return Ok(cart);
    }

    [HttpDelete("items/{cartDetailId}")]
    public async Task<IActionResult> RemoveItem(int cartDetailId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _cartService.RemoveItemAsync(userId, cartDetailId);
        if (!result) return NotFound(new { message = $"Cart item with ID {cartDetailId} not found" });
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _cartService.ClearCartAsync(userId);
        if (!result) return NotFound(new { message = "Cart not found" });
        return NoContent();
    }
} 
