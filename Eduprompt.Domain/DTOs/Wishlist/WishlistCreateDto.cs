using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Wishlist;

public class WishlistCreateDto
{
    [Required(ErrorMessage = "Package ID is required")]
    public int PackageID { get; set; }
    
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
} 
