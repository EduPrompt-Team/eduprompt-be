namespace Eduprompt.Domain.DTOs.Cart;

public class CartItemDto
{
    public int CartDetailId { get; set; }
    public int CartId { get; set; }
    public int PackageId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime AddedDate { get; set; }
    
    // Package info
    public string? PackageName { get; set; }
    public string? PackageDescription { get; set; }
} 
