using Eduprompt.Domain.DTOs.Cart;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eduprompt.API.Controllers;

/// <summary>
/// 🛒 Cart - Giỏ hàng của người dùng
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // All cart operations require authentication
[ApiExplorerSettings(GroupName = "18. Shopping Cart")]
[Produces("application/json")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// [AUTH] Get current user's cart
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var cart = await _cartService.GetUserCartAsync(userId);
        
        var cartDto = new CartDto
        {
            CartId = cart.CartId,
            UserId = cart.UserId,
            TotalItem = cart.TotalItem,
            CreatedDate = cart.CreatedDate,
            UpdatedDate = cart.UpdatedDate,
            Status = cart.Status,
            TotalPrice = cart.TotalPrice,
            Items = cart.Items?.Select(i => new CartItemDto
            {
                CartDetailId = i.CartDetailId,
                CartId = i.CartId,
                TemplateId = i.TemplateId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                SubTotal = i.SubTotal,
                AddedDate = i.AddedDate,
                Status = i.Status,
                TemplateName = i.TemplateName,
                TemplateDescription = i.TemplateDescription,
                PreviewUrl = i.PreviewUrl
            }).ToList()
        };

        return Ok(cartDto);
    }

    /// <summary>
    /// [AUTH] Add item to cart
    /// </summary>
    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemDto itemDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var addServiceDto = new AddCartItemServiceDto
            {
                TemplateId = itemDto.TemplateId,
                Quantity = itemDto.Quantity
            };

            var cart = await _cartService.AddItemAsync(userId, addServiceDto);
            
            var cartDto = new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                TotalItem = cart.TotalItem,
                CreatedDate = cart.CreatedDate,
                UpdatedDate = cart.UpdatedDate,
                Status = cart.Status,
                TotalPrice = cart.TotalPrice,
                Items = cart.Items?.Select(i => new CartItemDto
                {
                    CartDetailId = i.CartDetailId,
                    CartId = i.CartId,
                    TemplateId = i.TemplateId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    SubTotal = i.SubTotal,
                    AddedDate = i.AddedDate,
                    Status = i.Status,
                    TemplateName = i.TemplateName,
                    TemplateDescription = i.TemplateDescription,
                    PreviewUrl = i.PreviewUrl
                }).ToList()
            };

            return Ok(cartDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// [AUTH] Update cart item quantity
    /// </summary>
    [HttpPut("items/{cartDetailId}")]
    public async Task<IActionResult> UpdateItem(int cartDetailId, [FromBody] UpdateCartItemDto itemDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var updateServiceDto = new UpdateCartItemServiceDto
            {
                Quantity = itemDto.Quantity
            };

            var cart = await _cartService.UpdateItemAsync(userId, cartDetailId, updateServiceDto);
            
            var cartDto = new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                TotalItem = cart.TotalItem,
                CreatedDate = cart.CreatedDate,
                UpdatedDate = cart.UpdatedDate,
                Status = cart.Status,
                TotalPrice = cart.TotalPrice,
                Items = cart.Items?.Select(i => new CartItemDto
                {
                    CartDetailId = i.CartDetailId,
                    CartId = i.CartId,
                    TemplateId = i.TemplateId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    SubTotal = i.SubTotal,
                    AddedDate = i.AddedDate,
                    Status = i.Status,
                    TemplateName = i.TemplateName,
                    TemplateDescription = i.TemplateDescription,
                    PreviewUrl = i.PreviewUrl
                }).ToList()
            };

            return Ok(cartDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        // UnauthorizedAccessException handled by middleware
    }

    /// <summary>
    /// [AUTH] Remove item from cart
    /// </summary>
    [HttpDelete("items/{cartDetailId}")]
    public async Task<IActionResult> RemoveItem(int cartDetailId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _cartService.RemoveItemAsync(userId, cartDetailId);
        
        if (!result)
            return NotFound(new { message = $"Cart item with ID {cartDetailId} not found" });

        return NoContent();
        // UnauthorizedAccessException handled by middleware
    }

    /// <summary>
    /// [AUTH] Clear all items from cart
    /// </summary>
    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _cartService.ClearCartAsync(userId);
        
        if (!result)
            return NotFound(new { message = "Cart not found" });

        return NoContent();
    }
} 
