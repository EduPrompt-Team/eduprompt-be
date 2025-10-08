// using Eduprompt.Domain.DTOs.Payment; // Removed - Payment DTOs deleted

namespace Eduprompt.Domain.DTOs.Order;

public class OrderDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime OrderDate { get; set; }
    public string? Status { get; set; }
    
    // User info
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    
    // Order items
    public List<OrderItemDto>? Items { get; set; }
    
    // Payments
    // public List<PaymentDto>? Payments { get; set; } // Removed - Payment DTOs deleted
} 