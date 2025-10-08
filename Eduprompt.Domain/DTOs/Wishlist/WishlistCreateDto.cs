using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Wishlist;

public class WishlistCreateDto
{
    [Required(ErrorMessage = "Template ID is required")]
    public int TemplateId { get; set; }
    
    [StringLength(100, ErrorMessage = "Wishlist name cannot exceed 100 characters")]
    public string? WishlistName { get; set; }
} 
