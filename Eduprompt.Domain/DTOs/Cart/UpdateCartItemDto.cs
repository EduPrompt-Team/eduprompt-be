using System.ComponentModel.DataAnnotations;

namespace Eduprompt.Domain.DTOs.Cart;

public class UpdateCartItemDto
{
    [Range(0, 100, ErrorMessage = "Quantity must be between 0 and 100")]
    public int Quantity { get; set; }
} 
