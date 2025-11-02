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
    private readonly IWalletService _walletService;
    private readonly ITransactionRepository _transactionRepository;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IConfiguration configuration,
        IPaymentMethodRepository paymentMethodRepository,
        IWalletRepository walletRepository,
        IWalletService walletService,
        ITransactionRepository transactionRepository)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _configuration = configuration;
        _paymentMethodRepository = paymentMethodRepository;
        _walletRepository = walletRepository;
        _walletService = walletService;
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
                // Check if this is a wallet top-up (TxnRef starts with "WLT-")
                if (payment.TxnRef?.StartsWith("WLT-", StringComparison.OrdinalIgnoreCase) == true)
                {
                    // Handle wallet top-up
                    if (payment.UserId.HasValue)
                    {
                        await _walletService.AddFundsByUserIdAsync(payment.UserId.Value, payment.Amount);
                        
                        // Create transaction record for top-up
                        try
                        {
                            var methods = await _paymentMethodRepository.GetAllAsync();
                            var vnpMethod = methods.FirstOrDefault(m => (m.Provider ?? "").Equals("VNPay", StringComparison.OrdinalIgnoreCase) || (m.MethodName ?? "").Contains("vnp", StringComparison.OrdinalIgnoreCase));
                            if (vnpMethod != null)
                            {
                                var wallet = await _walletRepository.GetByUserIdAsync(payment.UserId.Value);
                                if (wallet != null)
                                {
                                    var trx = new Transaction
                                    {
                                        PaymentMethodId = vnpMethod.PaymentMethodId,
                                        WalletId = wallet.WalletId,
                                        OrderId = null, // Top-up doesn't have OrderId
                                        Amount = payment.Amount,
                                        TransactionType = "TopUp",
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
                // Check if this is a transaction payment (TxnRef starts with "TXN-")
                else if (payment.TxnRef?.StartsWith("TXN-", StringComparison.OrdinalIgnoreCase) == true)
                {
                    // Handle transaction payment - amount already processed, just create transaction record
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
                                    OrderId = null, // Transaction payment may not have OrderId
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
                // Regular order payment
                else if (payment.OrderId.HasValue && payment.OrderId.Value != 0)
                {
                    // Update order status
                    var order = await _orderRepository.GetByIdAsync(payment.OrderId.Value);
                    if (order != null)
                    {
                        order.Status = "Paid";
                        await _orderRepository.UpdateAsync(order);
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
                                        OrderId = payment.OrderId, // Nullable, can be null for wallet top-up
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

    public async Task<string> CreateVnpayUrlForWalletTopupAsync(int walletId, decimal amount, int userId, VnpayRequestServiceDto requestDto)
    {
        var wallet = await _walletRepository.GetByIdAsync(walletId);
        if (wallet == null) throw new InvalidOperationException("Wallet not found");
        if (wallet.UserId != userId) throw new UnauthorizedAccessException("Wallet does not belong to user");

        if (amount <= 0) throw new ArgumentException("Amount must be greater than 0", nameof(amount));

        var vnp = _configuration.GetSection("VNPay");
        var baseUrl = vnp["Url"] ?? string.Empty;
        var tmnCode = vnp["TmnCode"] ?? string.Empty;
        var returnUrl = requestDto.ReturnUrl ?? vnp["ReturnUrl"] ?? string.Empty;
        var hashSecret = vnp["HashSecret"] ?? string.Empty;

        var txnRef = $"WLT-{walletId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var vnpAmount = (long)(amount * 100);

        var nowGmt7 = DateTime.UtcNow.AddHours(7);
        var createDate = nowGmt7.ToString("yyyyMMddHHmmss");
        var ipAddr = string.IsNullOrWhiteSpace(requestDto.IpAddr) ? "127.0.0.1" : requestDto.IpAddr;

        var dict = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = tmnCode,
            ["vnp_Amount"] = vnpAmount.ToString(),
            ["vnp_CurrCode"] = "VND",
            ["vnp_TxnRef"] = txnRef,
            ["vnp_OrderInfo"] = $"wallet_topup_{walletId}",
            ["vnp_OrderType"] = "topup",
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

        // create pending payment record (OrderId = null for wallet top-up)
        var payment = new Payment
        {
            OrderId = null, // Nullable - wallet top-up doesn't require OrderId
            UserId = userId,
            Amount = amount,
            PaymentMethod = "Online",
            Provider = "VNPay",
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            TxnRef = txnRef
        };
        await _paymentRepository.CreateAsync(payment);

        return url;
    }

    public async Task<string> CreateVnpayUrlForTransactionAsync(int transactionId, int userId, VnpayRequestServiceDto requestDto)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        if (transaction == null) throw new InvalidOperationException("Transaction not found");

        var wallet = await _walletRepository.GetByIdAsync(transaction.WalletId);
        if (wallet == null || wallet.UserId != userId) throw new UnauthorizedAccessException("Transaction does not belong to user");

        var vnp = _configuration.GetSection("VNPay");
        var baseUrl = vnp["Url"] ?? string.Empty;
        var tmnCode = vnp["TmnCode"] ?? string.Empty;
        var returnUrl = requestDto.ReturnUrl ?? vnp["ReturnUrl"] ?? string.Empty;
        var hashSecret = vnp["HashSecret"] ?? string.Empty;

        var txnRef = $"TXN-{transactionId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var vnpAmount = (long)(transaction.Amount * 100);

        var nowGmt7 = DateTime.UtcNow.AddHours(7);
        var createDate = nowGmt7.ToString("yyyyMMddHHmmss");
        var ipAddr = string.IsNullOrWhiteSpace(requestDto.IpAddr) ? "127.0.0.1" : requestDto.IpAddr;

        var dict = new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = tmnCode,
            ["vnp_Amount"] = vnpAmount.ToString(),
            ["vnp_CurrCode"] = "VND",
            ["vnp_TxnRef"] = txnRef,
            ["vnp_OrderInfo"] = $"transaction_{transactionId}",
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

        // create pending payment record (OrderId = null for transaction payment)
        var payment = new Payment
        {
            OrderId = null, // Nullable - transaction payment may not require OrderId
            UserId = userId,
            Amount = transaction.Amount,
            PaymentMethod = "Online",
            Provider = "VNPay",
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            TxnRef = txnRef
        };
        await _paymentRepository.CreateAsync(payment);

        return url;
    }

    private static PaymentServiceDto Map(Payment p)
    {
        return new PaymentServiceDto
        {
            PaymentId = p.PaymentId,
            OrderId = p.OrderId ?? 0, // Map null to 0 for DTO compatibility
            PaymentMethod = p.PaymentMethod,
            Amount = p.Amount,
            PaymentDate = p.CreatedAt,
            Status = p.Status,
            VnpayTransactionId = p.TransactionNo,
            VnpayResponseCode = p.ResponseCode
        };
    }
}


