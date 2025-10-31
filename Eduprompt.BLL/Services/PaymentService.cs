using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;
using Microsoft.Extensions.Configuration;

namespace Eduprompt.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IConfiguration _configuration;

    public PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository, IConfiguration configuration)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _configuration = configuration;
    }

    public async Task<PaymentServiceDto?> GetByIdAsync(int id)
    {
        var p = await _paymentRepository.GetByIdAsync(id);
        return p == null ? null : Map(p);
    }

    public async Task<IEnumerable<PaymentServiceDto>> GetByOrderIdAsync(int orderId)
    {
        var list = await _paymentRepository.GetByOrderIdAsync(orderId);
        return list.Select(Map);
    }

    public async Task<IEnumerable<PaymentServiceDto>> GetAllPaymentsAsync()
    {
        var list = await _paymentRepository.GetAllAsync();
        return list.Select(Map);
    }

    public async Task<string> CreateVnpayPaymentUrlAsync(int orderId, int userId, VnpayRequestServiceDto requestDto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) throw new InvalidOperationException("Order not found");

        var vnp = _configuration.GetSection("VNPay");
        var baseUrl = vnp["Url"] ?? string.Empty;
        var tmnCode = vnp["TmnCode"] ?? string.Empty;
        var returnUrl = requestDto.ReturnUrl ?? vnp["ReturnUrl"] ?? string.Empty;

        var txnRef = $"ORD-{orderId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var amount = (long)(order.TotalAmount * 100);

        // Minimal URL (signing omitted here; add real signing later)
        var query = $"vnp_Version=2.1.0&vnp_Command=pay&vnp_TmnCode={tmnCode}&vnp_Amount={amount}&vnp_TxnRef={txnRef}&vnp_OrderInfo=order_{orderId}&vnp_ReturnUrl={Uri.EscapeDataString(returnUrl)}";
        var url = $"{baseUrl}?{query}";

        // create pending payment record
        var payment = new Payment
        {
            OrderId = orderId,
            UserId = userId,
            Amount = order.TotalAmount,
            PaymentMethod = "Online",
            Provider = "VNPay",
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            TxnRef = txnRef
        };
        await _paymentRepository.CreateAsync(payment);

        return url;
    }

    public async Task<PaymentServiceDto> ProcessVnpayCallbackAsync(VnpayCallbackServiceDto cb)
    {
        // Find payment by TxnRef
        // In absence of direct lookup, list and filter (could add repo method later)
        var all = await _paymentRepository.GetAllAsync();
        var payment = all.FirstOrDefault(p => p.TxnRef == cb.vnp_TxnRef) ?? throw new InvalidOperationException("Payment not found");

        payment.TransactionNo = cb.vnp_TransactionNo;
        payment.ResponseCode = cb.vnp_ResponseCode;
        payment.BankCode = cb.vnp_BankCode;
        payment.PayDate = cb.vnp_PayDate;
        payment.Status = cb.vnp_ResponseCode == "00" ? "Paid" : "Failed";
        payment.UpdatedAt = DateTime.UtcNow;

        await _paymentRepository.UpdateAsync(payment);

        return Map(payment);
    }

    public async Task<PaymentServiceDto> CreateManualPaymentAsync(int orderId, PaymentCreateServiceDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(orderId) ?? throw new InvalidOperationException("Order not found");
        var payment = new Payment
        {
            OrderId = orderId,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            Provider = dto.Provider,
            Status = "Paid",
            CreatedAt = DateTime.UtcNow
        };
        await _paymentRepository.CreateAsync(payment);
        return Map(payment);
    }

    public async Task<PaymentServiceDto> UpdatePaymentStatusAsync(int paymentId, string status)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId) ?? throw new InvalidOperationException("Payment not found");
        payment.Status = status;
        payment.UpdatedAt = DateTime.UtcNow;
        await _paymentRepository.UpdateAsync(payment);
        return Map(payment);
    }

    private static PaymentServiceDto Map(Payment p)
    {
        return new PaymentServiceDto
        {
            PaymentId = p.PaymentId,
            OrderId = p.OrderId,
            PaymentMethod = p.PaymentMethod,
            Amount = p.Amount,
            PaymentDate = p.CreatedAt,
            Status = p.Status,
            VnpayTransactionId = p.TransactionNo,
            VnpayResponseCode = p.ResponseCode
        };
    }
}


