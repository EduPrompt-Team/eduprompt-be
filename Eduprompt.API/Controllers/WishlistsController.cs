using Eduprompt.Domain.DTOs.Wishlist;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eduprompt.API.Controllers;

/// <summary>
/// ❤️ Wishlists - Danh sách yêu thích của người dùng
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // All wishlist operations require authentication
[ApiExplorerSettings(GroupName = "08. Wishlists")]
[Produces("application/json")]
public class WishlistsController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistsController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    /// <summary>
    /// [AUTH] Get current user's wishlist
    /// </summary>
    [HttpGet("my-wishlist")]
    public async Task<IActionResult> GetMyWishlist()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var wishlists = await _wishlistService.GetUserWishlistAsync(userId);
        
        var wishlistDtos = wishlists.Select(w => new WishlistDto
        {
            WishlistId = w.WishlistId,
            UserId = w.UserId,
            TemplateId = w.TemplateId,
            WishlistName = w.WishlistName,
            CreatedDate = w.CreatedDate,
            Status = w.Status,
            UserName = w.UserName,
            TemplateName = w.TemplateName,
            TemplateDescription = w.TemplateDescription,
            TemplatePrice = w.TemplatePrice,
            TemplatePreviewUrl = w.TemplatePreviewUrl
        });

        return Ok(wishlistDtos);
    }

    /// <summary>
    /// [AUTH] Add template to wishlist
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddToWishlist([FromBody] WishlistCreateDto wishlistDto)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var createServiceDto = new WishlistCreateServiceDto
            {
                TemplateId = wishlistDto.TemplateId,
                WishlistName = wishlistDto.WishlistName
            };

            var wishlist = await _wishlistService.AddToWishlistAsync(userId, createServiceDto);
            
            var resultDto = new WishlistDto
            {
                WishlistId = wishlist.WishlistId,
                UserId = wishlist.UserId,
                TemplateId = wishlist.TemplateId,
                WishlistName = wishlist.WishlistName,
                CreatedDate = wishlist.CreatedDate,
                Status = wishlist.Status,
                TemplateName = wishlist.TemplateName,
                TemplateDescription = wishlist.TemplateDescription,
                TemplatePrice = wishlist.TemplatePrice,
                TemplatePreviewUrl = wishlist.TemplatePreviewUrl
            };

            return CreatedAtAction(nameof(GetMyWishlist), resultDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// [AUTH] Remove template from wishlist
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromWishlist(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _wishlistService.RemoveFromWishlistAsync(id, userId);
        
        if (!result)
            return NotFound(new { message = $"Wishlist item with ID {id} not found" });

        return NoContent();
        // UnauthorizedAccessException handled by middleware
    }

    /// <summary>
    /// [AUTH] Check if template is in wishlist
    /// </summary>
    [HttpGet("check/{templateId}")]
    public async Task<IActionResult> CheckWishlist(int templateId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, templateId);
        
        return Ok(new { templateId, isInWishlist });
    }
} 
