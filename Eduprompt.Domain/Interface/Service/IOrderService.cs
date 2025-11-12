namespace Eduprompt.Domain.Interface.Service;

public interface IOrderService
{
    Task<OrderServiceDto> CreateOrderFromCartAsync(int userId, string? notes);
    Task<OrderServiceDto?> GetByIdAsync(int orderId, int userId);
    Task<OrderServiceDto?> GetByIdAdminAsync(int orderId); // Admin
    Task<IEnumerable<OrderServiceDto>> GetUserOrdersAsync(int userId);
    Task<IEnumerable<OrderServiceDto>> GetAllOrdersAsync(); // Admin
    Task<OrderServiceDto> CancelOrderAsync(int orderId, int userId);
    Task<OrderServiceDto> UpdateOrderStatusAsync(int orderId, string status); // Admin
    Task<OrderServiceDto> PayOrderWithWalletAsync(int orderId, int userId); // Pay order directly with wallet
}

public class OrderServiceDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime OrderDate { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public List<OrderItemServiceDto>? Items { get; set; }
    
    // Payments
    public List<PaymentServiceDto>? Payments { get; set; }
}

public class OrderItemServiceDto
{
    public int OrderDetailId { get; set; }
    public int OrderId { get; set; }
    public int TemplateId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal SubTotal { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? Status { get; set; }
    public string? TemplateName { get; set; }
    public string? TemplateDescription { get; set; }
    public string? PreviewUrl { get; set; }
} 