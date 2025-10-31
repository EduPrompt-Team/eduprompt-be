namespace Eduprompt.Domain.Interface.Service;

public interface IPaymentService
{
    Task<PaymentServiceDto?> GetByIdAsync(int id);
    Task<IEnumerable<PaymentServiceDto>> GetByOrderIdAsync(int orderId);
    Task<IEnumerable<PaymentServiceDto>> GetAllPaymentsAsync(); // Admin only
    
    // VNPAY Payment Flow
    Task<string> CreateVnpayPaymentUrlAsync(int orderId, int userId, VnpayRequestServiceDto requestDto);
    Task<PaymentServiceDto> ProcessVnpayCallbackAsync(VnpayCallbackServiceDto callbackDto);
    
    // Manual Payment (for COD or other methods)
    Task<PaymentServiceDto> CreateManualPaymentAsync(int orderId, PaymentCreateServiceDto paymentDto);
    Task<PaymentServiceDto> UpdatePaymentStatusAsync(int paymentId, string status); // Admin only
}

// Service DTOs
public class PaymentServiceDto
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Status { get; set; }
    public string? OrderNumber { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public string? VnpayTransactionId { get; set; }
    public string? VnpayResponseCode { get; set; }
}

public class PaymentCreateServiceDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Provider { get; set; } = "VNPay";
}

public class VnpayRequestServiceDto
{
    public string? BankCode { get; set; }
    public string Language { get; set; } = "vn";
    public string? ReturnUrl { get; set; }
}

public class VnpayCallbackServiceDto
{
    public string vnp_TmnCode { get; set; } = string.Empty;
    public string vnp_Amount { get; set; } = string.Empty;
    public string vnp_BankCode { get; set; } = string.Empty;
    public string vnp_BankTranNo { get; set; } = string.Empty;
    public string vnp_CardType { get; set; } = string.Empty;
    public string vnp_PayDate { get; set; } = string.Empty;
    public string vnp_OrderInfo { get; set; } = string.Empty;
    public string vnp_TransactionNo { get; set; } = string.Empty;
    public string vnp_ResponseCode { get; set; } = string.Empty;
    public string vnp_TransactionStatus { get; set; } = string.Empty;
    public string vnp_TxnRef { get; set; } = string.Empty;
    public string vnp_SecureHash { get; set; } = string.Empty;
} 