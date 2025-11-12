using Eduprompt.Domain.DTOs.Wishlist;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Eduprompt.API.Controllers;

/// <summary>
/// User wishlist management for favorite prompt templates (StorageTemplates)
/// </summary>
[ApiController]
[Route("api/wishlists")]
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

    /// <summary>
    /// Get current user's wishlist with StorageTemplate details
    /// </summary>
    [HttpGet("my-wishlist")]
    public async Task<IActionResult> GetMyWishlist()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var wishlists = await _wishlistService.GetByUserIdAsync(userId);
        return Ok(wishlists);
    }

    /// <summary>
    /// Add StorageTemplate to wishlist
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddToWishlist([FromBody] WishlistCreateDto wishlistDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var created = await _wishlistService.CreateAsync(userId, wishlistDto);
        return CreatedAtAction(nameof(GetMyWishlist), created);
    }

    /// <summary>
    /// Remove wishlist item by ID
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromWishlist(int id)
    {
        var result = await _wishlistService.DeleteAsync(id);
        if (!result)
            return NotFound(new { message = $"Wishlist item with ID {id} not found" });
        return NoContent();
    }

    /// <summary>
    /// Remove wishlist item by StorageId
    /// </summary>
    [HttpDelete("by-storage/{storageId}")]
    public async Task<IActionResult> RemoveFromWishlistByStorageId(int storageId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _wishlistService.DeleteByStorageIdAsync(userId, storageId);
        if (!result)
            return NotFound(new { message = $"StorageTemplate with ID {storageId} not found in your wishlist" });
        return NoContent();
    }

    /// <summary>
    /// Check if Package is in wishlist (legacy endpoint for backward compatibility)
    /// </summary>
    [HttpGet("check/package/{packageId}")]
    public async Task<IActionResult> CheckWishlistByPackage(int packageId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, packageId);
        return Ok(new { PackageId = packageId, isInWishlist });
    }

    /// <summary>
    /// Check if StorageTemplate is in wishlist
    /// </summary>
    [HttpGet("check/{storageId}")]
    public async Task<IActionResult> CheckWishlist(int storageId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isInWishlist = await _wishlistService.IsInWishlistByStorageIdAsync(userId, storageId);
        return Ok(new { StorageId = storageId, isInWishlist });
    }
} 
