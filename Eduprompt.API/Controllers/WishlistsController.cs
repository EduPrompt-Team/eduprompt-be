using Eduprompt.Domain.DTOs.Wishlist;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eduprompt.API.Controllers;

/// <summary>
/// User wishlist management for favorite packages
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[ApiExplorerSettings(GroupName = "08. Wishlists")]
[Produces("application/json")]
public class WishlistsController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistsController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpGet("my-wishlist")]
    public async Task<IActionResult> GetMyWishlist()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var wishlists = await _wishlistService.GetByUserIdAsync(userId);
        return Ok(wishlists);
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishlist([FromBody] WishlistCreateDto wishlistDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var created = await _wishlistService.CreateAsync(userId, wishlistDto);
        return CreatedAtAction(nameof(GetMyWishlist), created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromWishlist(int id)
    {
        var result = await _wishlistService.DeleteAsync(id);
        if (!result)
            return NotFound(new { message = $"Wishlist item with ID {id} not found" });
        return NoContent();
    }

    [HttpGet("check/{packageId}")]
    public async Task<IActionResult> CheckWishlist(int packageId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, packageId);
        return Ok(new { packageId, isInWishlist });
    }
} 
