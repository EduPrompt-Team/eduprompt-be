using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Wishlist;

public class WishlistCreateDto
{
    // Optional - for backward compatibility with existing Package-based wishlists
    public int? PackageId { get; set; }
    
    // Required - ID of StorageTemplate (prompt template) to add to wishlist
    [Required(ErrorMessage = "Storage ID is required")]
    public int StorageId { get; set; }
    
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
} 
