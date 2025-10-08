namespace Eduprompt.Domain.DTOs.Cart;

public class CartDto
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public int? TotalItem { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? Status { get; set; }
    
    // Calculated
    public decimal TotalPrice { get; set; }
    
    // Items in cart
    public List<CartItemDto>? Items { get; set; }
} 
