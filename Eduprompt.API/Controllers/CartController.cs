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
    /// Get current user's shopping cart
    /// </summary>
    /// <returns>User's cart with all items</returns>
    /// <response code="200">Cart retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
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
    /// Add item to shopping cart
    /// </summary>
    /// <param name="itemDto">Item details to add to cart</param>
    /// <returns>Updated cart with new item</returns>
    /// <response code="200">Item added to cart successfully</response>
    /// <response code="400">Invalid item data</response>
    /// <response code="401">User not authenticated</response>
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
    /// Update quantity of cart item
    /// </summary>
    /// <param name="cartDetailId">Cart detail ID to update</param>
    /// <param name="itemDto">Updated item quantity</param>
    /// <returns>Updated cart with modified item</returns>
    /// <response code="200">Item updated successfully</response>
    /// <response code="400">Invalid item data</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Cart item not found</response>
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
    /// Remove item from shopping cart
    /// </summary>
    /// <param name="cartDetailId">Cart detail ID to remove</param>
    /// <returns>No content</returns>
    /// <response code="204">Item removed successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Cart item not found</response>
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
    /// Clear all items from shopping cart
    /// </summary>
    /// <returns>No content</returns>
    /// <response code="204">Cart cleared successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Cart not found</response>
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
