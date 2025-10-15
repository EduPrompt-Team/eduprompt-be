using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Cart;

public class AddCartItemDto
{
    [Required(ErrorMessage = "Package ID is required")]
    public int PackageID { get; set; }
    
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
    public int Quantity { get; set; } = 1;
} 
