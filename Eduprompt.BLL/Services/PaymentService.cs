using Eduprompt.Domain.Entities;
using Eduprompt.Domain.Interface.Repository;
using Eduprompt.Domain.Interface.Service;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Json;

namespace Eduprompt.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IConfiguration _configuration;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IConfiguration configuration,
        IPaymentMethodRepository paymentMethodRepository,
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _configuration = configuration;
        _paymentMethodRepository = paymentMethodRepository;
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
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
        var hashSecret = vnp["HashSecret"] ?? string.Empty;

        var txnRef = $"ORD-{orderId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var amount = (long)(order.TotalAmount * 100);

        var nowGmt7 = DateTime.UtcNow.AddHours(7);
        var createDate = nowGmt7.ToString("yyyyMMddHHmmss");
        var ipAddr = string.IsNullOrWhiteSpace(requestDto.IpAddr) ? "127.0.0.1" : requestDto.IpAddr;

        var dict = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = tmnCode,
            ["vnp_Amount"] = amount.ToString(),
            ["vnp_CurrCode"] = "VND",
            ["vnp_TxnRef"] = txnRef,
            ["vnp_OrderInfo"] = $"order_{orderId}",
            ["vnp_OrderType"] = "other",
            ["vnp_Locale"] = requestDto.Language ?? "vn",
            ["vnp_CreateDate"] = createDate,
            ["vnp_IpAddr"] = ipAddr,
            ["vnp_ReturnUrl"] = returnUrl
        };
        if (!string.IsNullOrWhiteSpace(requestDto.BankCode))
        {
            dict["vnp_BankCode"] = requestDto.BankCode!;
        }

        // build signData (no URL-encode for signing)
        var raw = string.Join("&", dict.Select(kv => $"{kv.Key}={kv.Value}"));
        using var hmac = new System.Security.Cryptography.HMACSHA512(System.Text.Encoding.UTF8.GetBytes(hashSecret));
        var signature = BitConverter.ToString(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(raw))).Replace("-", string.Empty).ToLowerInvariant();

        // build final url with URL-encoded values plus vnp_SecureHash
        var encoded = string.Join("&", dict.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
        var url = $"{baseUrl}?{encoded}&vnp_SecureHash={signature}";

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
        var vnp = _configuration.GetSection("VNPay");
        var hashSecret = vnp["HashSecret"] ?? string.Empty;

        // Verify signature
        var fields = new SortedDictionary<string, string>(StringComparer.Ordinal);
        void add(string key, string value) { if (!string.IsNullOrEmpty(value)) fields[key] = value; }
        add("vnp_Amount", cb.vnp_Amount);
        add("vnp_BankCode", cb.vnp_BankCode);
        add("vnp_BankTranNo", cb.vnp_BankTranNo);
        add("vnp_CardType", cb.vnp_CardType);
        add("vnp_OrderInfo", cb.vnp_OrderInfo);
        add("vnp_PayDate", cb.vnp_PayDate);
        add("vnp_ResponseCode", cb.vnp_ResponseCode);
        add("vnp_TmnCode", cb.vnp_TmnCode);
        add("vnp_TransactionNo", cb.vnp_TransactionNo);
        add("vnp_TransactionStatus", cb.vnp_TransactionStatus);
        add("vnp_TxnRef", cb.vnp_TxnRef);

        var raw = string.Join("&", fields.Select(kv => $"{kv.Key}={kv.Value}"));
        using var hmac = new System.Security.Cryptography.HMACSHA512(System.Text.Encoding.UTF8.GetBytes(hashSecret));
        var signed = BitConverter.ToString(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(raw))).Replace("-", string.Empty).ToLowerInvariant();
        var secure = (cb.vnp_SecureHash ?? string.Empty).ToLowerInvariant();

        var all = await _paymentRepository.GetAllAsync();
        var payment = all.FirstOrDefault(p => p.TxnRef == cb.vnp_TxnRef) ?? throw new InvalidOperationException("Payment not found");

        if (signed != secure)
        {
            payment.Status = "Failed";
        }
        else
        {
            payment.TransactionNo = cb.vnp_TransactionNo;
            payment.ResponseCode = cb.vnp_ResponseCode;
            payment.BankCode = cb.vnp_BankCode;
            payment.PayDate = cb.vnp_PayDate;
            var success = cb.vnp_ResponseCode == "00";
            payment.Status = success ? "Paid" : "Failed";

            if (success)
            {
                // Update order status
                if (payment.OrderId != 0)
                {
                    var order = await _orderRepository.GetByIdAsync(payment.OrderId);
                    if (order != null)
                    {
                        order.Status = "Paid";
                        await _orderRepository.UpdateAsync(order);
                    }
                }

                // Create transaction if possible (optional, no wallet balance mutation)
                try
                {
                    var methods = await _paymentMethodRepository.GetAllAsync();
                    var vnpMethod = methods.FirstOrDefault(m => (m.Provider ?? "").Equals("VNPay", StringComparison.OrdinalIgnoreCase) || (m.MethodName ?? "").Contains("vnp", StringComparison.OrdinalIgnoreCase));
                    if (vnpMethod != null && payment.UserId.HasValue)
                    {
                        var wallet = await _walletRepository.GetByUserIdAsync(payment.UserId.Value);
                        if (wallet != null)
                        {
                            var trx = new Transaction
                            {
                                PaymentMethodId = vnpMethod.PaymentMethodId,
                                WalletId = wallet.WalletId,
                                OrderId = payment.OrderId,
                                Amount = payment.Amount,
                                TransactionType = "ExternalPayment",
                                TransactionDate = DateTime.UtcNow,
                                Status = "Completed",
                                TransactionReference = payment.TransactionNo ?? payment.TxnRef
                            };
                            await _transactionRepository.CreateAsync(trx);
                        }
                    }
                }
                catch { /* ignore optional transaction creation errors */ }
            }
        }
        payment.UpdatedAt = DateTime.UtcNow;

        await _paymentRepository.UpdateAsync(payment);

        return Map(payment);
    }

    public async Task<object> QueryVnpayTransactionAsync(VnpayQueryRequestDto request)
    {
        var vnp = _configuration.GetSection("VNPay");
        var tmnCode = vnp["TmnCode"] ?? string.Empty;
        var hashSecret = vnp["HashSecret"] ?? string.Empty;
        var apiUrl = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";
        var ip = string.IsNullOrWhiteSpace(request.IpAddr) ? "127.0.0.1" : request.IpAddr;

        var now = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss");
        var payload = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_RequestId"] = Guid.NewGuid().ToString("N"),
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "querydr",
            ["vnp_TmnCode"] = tmnCode,
            ["vnp_TxnRef"] = request.TxnRef,
            ["vnp_OrderInfo"] = request.OrderInfo ?? ("order_" + request.TxnRef),
            ["vnp_TransactionDate"] = request.TransactionDate,
            ["vnp_CreateDate"] = now,
            ["vnp_IpAddr"] = ip
        };
        var raw = string.Join("&", payload.Select(kv => $"{kv.Key}={kv.Value}"));
        using var hmac = new System.Security.Cryptography.HMACSHA512(System.Text.Encoding.UTF8.GetBytes(hashSecret));
        var sign = BitConverter.ToString(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(raw))).Replace("-", string.Empty).ToLowerInvariant();

        var obj = payload.ToDictionary(k => k.Key, v => (object)v.Value);
        obj["vnp_SecureHash"] = sign;

        using var http = new HttpClient();
        var res = await http.PostAsJsonAsync(apiUrl, obj);
        var json = await res.Content.ReadAsStringAsync();
        return new { StatusCode = (int)res.StatusCode, Body = json };
    }

    public async Task<object> RefundVnpayTransactionAsync(VnpayRefundRequestDto request)
    {
        var vnp = _configuration.GetSection("VNPay");
        var tmnCode = vnp["TmnCode"] ?? string.Empty;
        var hashSecret = vnp["HashSecret"] ?? string.Empty;
        var apiUrl = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";
        var ip = string.IsNullOrWhiteSpace(request.IpAddr) ? "127.0.0.1" : request.IpAddr;
        var now = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss");

        var payload = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_RequestId"] = Guid.NewGuid().ToString("N"),
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "refund",
            ["vnp_TmnCode"] = tmnCode,
            ["vnp_TxnRef"] = request.TxnRef,
            ["vnp_Amount"] = request.Amount,
            ["vnp_OrderInfo"] = "refund_" + request.TxnRef,
            ["vnp_TransactionDate"] = request.TransactionDate,
            ["vnp_CreateBy"] = request.CreateBy,
            ["vnp_CreateDate"] = now,
            ["vnp_IpAddr"] = ip
        };
        var raw = string.Join("&", payload.Select(kv => $"{kv.Key}={kv.Value}"));
        using var hmac = new System.Security.Cryptography.HMACSHA512(System.Text.Encoding.UTF8.GetBytes(hashSecret));
        var sign = BitConverter.ToString(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(raw))).Replace("-", string.Empty).ToLowerInvariant();

        var obj = payload.ToDictionary(k => k.Key, v => (object)v.Value);
        obj["vnp_SecureHash"] = sign;

        using var http = new HttpClient();
        var res = await http.PostAsJsonAsync(apiUrl, obj);
        var json = await res.Content.ReadAsStringAsync();
        return new { StatusCode = (int)res.StatusCode, Body = json };
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


