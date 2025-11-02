# 💳 GIẢI THÍCH CHI TIẾT LUỒNG THANH TOÁN VNPAY

## 📋 TỔNG QUAN

Luồng thanh toán VNPay gồm **3 loại payment**:
1. **Order Payment** - Thanh toán đơn hàng
2. **Wallet Top-up** - Nạp tiền vào ví
3. **Transaction Payment** - Thanh toán transaction

Tất cả đều sử dụng **VNPay Payment Gateway** với flow tương tự, chỉ khác ở **bước xử lý sau khi thanh toán thành công**.

---

## 🔄 LUỒNG THANH TOÁN TỔNG QUÁT

```
┌─────────────────────────────────────────────────────────────┐
│                    VNPAY PAYMENT FLOW                        │
└─────────────────────────────────────────────────────────────┘

1. Frontend → Backend: Tạo Payment URL
   ↓
2. Backend: Tạo Payment record (Status = "Pending")
   ↓
3. Backend → Frontend: Trả về VNPay payment URL
   ↓
4. Frontend: Redirect user đến VNPay
   ↓
5. User: Thanh toán trên VNPay website
   ↓
6. VNPay: Xử lý thanh toán
   ↓
7a. VNPay → Frontend: Callback (Browser redirect - GET)
7b. VNPay → Backend: IPN (Server-to-server - POST)
   ↓
8. Backend: Verify signature & Xử lý kết quả
   ↓
9. Backend: Cập nhật Payment status
   ↓
10. Backend: Xử lý business logic (theo loại payment)
```

---

## 📝 CHI TIẾT TỪNG BƯỚC

### **BƯỚC 1: Frontend yêu cầu tạo Payment URL**

#### **1.1. Wallet Top-up Example:**

```typescript
// Frontend code
POST /api/payments/wallets/1/topup
Body: {
  "amount": 200000,        // 200,000 VND
  "bankCode": "NCB",       // Optional
  "language": "vn",        // Optional
  "returnUrl": "http://localhost:5173/wallet/topup/callback"
}
```

#### **1.2. Backend xử lý (PaymentService.cs - Line 371-436):**

```csharp
public async Task<string> CreateVnpayUrlForWalletTopupAsync(...)
{
    // 1. Validate wallet
    var wallet = await _walletRepository.GetByIdAsync(walletId);
    if (wallet == null) throw new InvalidOperationException("Wallet not found");
    if (wallet.UserId != userId) throw new UnauthorizedAccessException(...);
    if (amount <= 0) throw new ArgumentException(...);
    
    // 2. Lấy VNPay config từ appsettings.json
    var vnp = _configuration.GetSection("VNPay");
    var baseUrl = vnp["Url"];          // https://sandbox.vnpayment.vn/paymentv2/vpcpay.html
    var tmnCode = vnp["TmnCode"];      // Terminal code từ VNPay
    var hashSecret = vnp["HashSecret"]; // Secret key để tạo signature
    var returnUrl = requestDto.ReturnUrl ?? vnp["ReturnUrl"];
    
    // 3. Tạo TxnRef (Transaction Reference) - Unique identifier
    var txnRef = $"WLT-{walletId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
    // Format: "WLT-1-20250117123456"
    // WLT = Wallet Top-up prefix
    
    // 4. Convert amount sang format VNPay (x100)
    var vnpAmount = (long)(amount * 100);  // 200000 → 20000000 (VNPay format)
    
    // 5. Tạo payment data dictionary
    var dict = new SortedDictionary<string, string>
    {
        ["vnp_Version"] = "2.1.0",        // VNPay API version
        ["vnp_Command"] = "pay",           // Command: thanh toán
        ["vnp_TmnCode"] = tmnCode,         // Merchant terminal code
        ["vnp_Amount"] = vnpAmount.ToString(), // Amount * 100
        ["vnp_CurrCode"] = "VND",          // Currency: VND
        ["vnp_TxnRef"] = txnRef,           // Transaction reference
        ["vnp_OrderInfo"] = $"wallet_topup_{walletId}", // Order info
        ["vnp_OrderType"] = "topup",       // Order type
        ["vnp_Locale"] = "vn",             // Language
        ["vnp_CreateDate"] = createDate,    // yyyyMMddHHmmss GMT+7
        ["vnp_IpAddr"] = ipAddr,           // User IP
        ["vnp_ReturnUrl"] = returnUrl       // Callback URL
    };
    
    // 6. TẠO SIGNATURE (HMAC-SHA512)
    // Bước quan trọng: VNPay dùng signature để verify request hợp lệ
    var raw = string.Join("&", dict.Select(kv => $"{kv.Key}={kv.Value}"));
    // Ví dụ: "vnp_Amount=20000000&vnp_Command=pay&vnp_CreateDate=20250117123456&..."
    
    using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hashSecret));
    var signature = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(raw)))
        .Replace("-", "")
        .ToLowerInvariant();
    // Signature = HMAC-SHA512(hashSecret, rawData)
    
    // 7. BUILD FINAL URL
    var encoded = string.Join("&", dict.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
    var url = $"{baseUrl}?{encoded}&vnp_SecureHash={signature}";
    // Ví dụ: https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_Amount=20000000&...&vnp_SecureHash=abc123...
    
    // 8. TẠO PAYMENT RECORD trong database
    var payment = new Payment
    {
        OrderId = null,        // Wallet top-up không có OrderId
        UserId = userId,
        Amount = amount,        // 200000 VND
        PaymentMethod = "Online",
        Provider = "VNPay",
        Status = "Pending",     // ⚠️ Chưa thanh toán
        CreatedAt = DateTime.UtcNow,
        TxnRef = txnRef        // Lưu để match với callback sau
    };
    await _paymentRepository.CreateAsync(payment);
    
    // 9. RETURN URL cho frontend
    return url;
}
```

**Kết quả:** Frontend nhận được VNPay payment URL

---

### **BƯỚC 2: Frontend redirect user đến VNPay**

```typescript
// Frontend code
const { url } = await response.json();
window.location.href = url;  // Redirect browser
```

User được chuyển đến trang VNPay để thanh toán.

---

### **BƯỚC 3: User thanh toán trên VNPay**

- User nhập thông tin thẻ/ngân hàng
- VNPay xử lý thanh toán
- Sau khi xong, VNPay sẽ:
  - **Redirect browser** về `ReturnUrl` (Callback)
  - **Gọi IPN URL** (Server-to-server notification)

---

### **BƯỚC 4: VNPay Callback (Browser Redirect)**

#### **4.1. VNPay redirect về:**

```
GET /api/payments/vnpay-callback?
  vnp_Amount=20000000&
  vnp_BankCode=NCB&
  vnp_TransactionNo=12345678&
  vnp_ResponseCode=00&          // "00" = Success
  vnp_TxnRef=WLT-1-20250117123456&
  vnp_SecureHash=abc123...&
  ...
```

#### **4.2. Backend xử lý (PaymentService.cs - Line 120-267):**

```csharp
public async Task<PaymentServiceDto> ProcessVnpayCallbackAsync(VnpayCallbackServiceDto cb)
{
    // 1. Lấy hashSecret để verify
    var hashSecret = _configuration["VNPay:HashSecret"];
    
    // 2. TẠO SIGNATURE TỪ CALLBACK DATA
    var fields = new SortedDictionary<string, string>();
    fields["vnp_Amount"] = cb.vnp_Amount;
    fields["vnp_BankCode"] = cb.vnp_BankCode;
    fields["vnp_BankTranNo"] = cb.vnp_BankTranNo;
    fields["vnp_ResponseCode"] = cb.vnp_ResponseCode;
    fields["vnp_TxnRef"] = cb.vnp_TxnRef;
    // ... các field khác (KHÔNG bao gồm vnp_SecureHash)
    
    var raw = string.Join("&", fields.Select(kv => $"{kv.Key}={kv.Value}"));
    var signed = HMACSHA512(hashSecret, raw);
    
    // 3. VERIFY SIGNATURE
    // So sánh signature tự tính với signature từ VNPay
    if (signed != cb.vnp_SecureHash)
    {
        // Signature không hợp lệ → Có thể bị giả mạo
        payment.Status = "Failed";
    }
    else
    {
        // Signature hợp lệ → Xử lý tiếp
        
        // 4. TÌM PAYMENT RECORD theo TxnRef
        var payment = await _paymentRepository.GetAllAsync()
            .FirstOrDefault(p => p.TxnRef == cb.vnp_TxnRef);
        
        // 5. LƯU THÔNG TIN TỪ VNPAY
        payment.TransactionNo = cb.vnp_TransactionNo;  // Transaction ID từ VNPay
        payment.ResponseCode = cb.vnp_ResponseCode;    // "00" = Success
        payment.BankCode = cb.vnp_BankCode;
        payment.PayDate = cb.vnp_PayDate;
        
        // 6. CHECK PAYMENT SUCCESS
        var success = cb.vnp_ResponseCode == "00";
        payment.Status = success ? "Paid" : "Failed";
        
        // 7. XỬ LÝ BUSINESS LOGIC THEO LOẠI PAYMENT
        if (success)
        {
            // === WALLET TOP-UP ===
            if (payment.TxnRef?.StartsWith("WLT-") == true)
            {
                // Nạp tiền vào wallet
                await _walletService.AddFundsByUserIdAsync(
                    payment.UserId.Value, 
                    payment.Amount
                );
                // Ví dụ: wallet.balance += 200000
                
                // Tạo Transaction record
                var trx = new Transaction
                {
                    PaymentMethodId = vnpMethod.PaymentMethodId,
                    WalletId = wallet.WalletId,
                    OrderId = null,
                    Amount = payment.Amount,
                    TransactionType = "TopUp",  // ⭐ Loại transaction
                    TransactionDate = DateTime.UtcNow,
                    Status = "Completed",
                    TransactionReference = payment.TransactionNo
                };
                await _transactionRepository.CreateAsync(trx);
            }
            
            // === TRANSACTION PAYMENT ===
            else if (payment.TxnRef?.StartsWith("TXN-") == true)
            {
                // Chỉ tạo Transaction record, không nạp tiền
                // (Vì đây là thanh toán transaction, không phải top-up)
            }
            
            // === ORDER PAYMENT ===
            else if (payment.OrderId.HasValue)
            {
                // Cập nhật Order status = "Paid"
                var order = await _orderRepository.GetByIdAsync(payment.OrderId.Value);
                order.Status = "Paid";
                await _orderRepository.UpdateAsync(order);
                
                // Tạo Transaction record (optional)
            }
        }
        
        // 8. CẬP NHẬT PAYMENT STATUS
        payment.UpdatedAt = DateTime.UtcNow;
        await _paymentRepository.UpdateAsync(payment);
    }
    
    return Map(payment);  // Return payment info
}
```

---

### **BƯỚC 5: VNPay IPN (Server-to-Server Notification)**

**Song song với Callback**, VNPay cũng gọi IPN endpoint:

```
POST /api/payments/vnpay-ipn
Content-Type: application/x-www-form-urlencoded

vnp_Amount=20000000&
vnp_ResponseCode=00&
vnp_TxnRef=WLT-1-20250117123456&
...
```

**Mục đích:** Đảm bảo backend nhận được notification ngay cả khi user đóng browser trước khi callback hoàn thành.

**Xử lý:**
```csharp
[HttpPost("vnpay-ipn")]
[AllowAnonymous]
public async Task<IActionResult> VnpayIpn([FromForm] VnpayCallbackServiceDto cb)
{
    try
    {
        var _ = await _paymentService.ProcessVnpayCallbackAsync(cb);
        return Ok(new { RspCode = "00", Message = "Confirm Success" });
    }
    catch
    {
        return Ok(new { RspCode = "97", Message = "Invalid signature or data" });
    }
}
```

**Lưu ý:** IPN luôn return 200 OK với `RspCode` để VNPay biết đã nhận được.

---

## 🔐 SIGNATURE (CHỮ KÝ SỐ) - QUAN TRỌNG NHẤT

### **Tại sao cần Signature?**

- **Bảo mật**: Đảm bảo request đến từ VNPay thực sự, không bị giả mạo
- **Toàn vẹn dữ liệu**: Đảm bảo dữ liệu không bị sửa đổi trong quá trình truyền

### **Cách tạo Signature:**

#### **Khi tạo Payment URL (Request):**

```
1. Tạo SortedDictionary (sắp xếp theo key alphabetically)
2. Convert thành string: "vnp_Amount=20000000&vnp_Command=pay&..."
3. Bỏ vnp_SecureHash (chưa có)
4. Tính HMAC-SHA512(hashSecret, rawString)
5. Convert hex → lowercase string
6. Thêm vào URL: ...&vnp_SecureHash=abc123...
```

#### **Khi nhận Callback (Verify):**

```
1. Lấy tất cả params từ VNPay (TRỪ vnp_SecureHash)
2. Tạo SortedDictionary (same order)
3. Convert thành string (same format)
4. Tính HMAC-SHA512(hashSecret, rawString)
5. So sánh với vnp_SecureHash từ VNPay
6. Nếu khác → Signature invalid → REJECT
```

---

## 📊 SO SÁNH 3 LOẠI PAYMENT

| Loại | TxnRef Prefix | OrderId | Sau khi thành công |
|------|--------------|---------|-------------------|
| **Wallet Top-up** | `WLT-{walletId}-{timestamp}` | `NULL` | ✅ Nạp tiền vào wallet<br>✅ Tạo Transaction (Type="TopUp") |
| **Transaction Payment** | `TXN-{transactionId}-{timestamp}` | `NULL` | ✅ Tạo Transaction (Type="ExternalPayment") |
| **Order Payment** | `ORD-{orderId}-{timestamp}` | `orderId` | ✅ Update Order status = "Paid"<br>✅ Tạo Transaction (Type="ExternalPayment") |

---

## 🔍 CHI TIẾT VNPAY PARAMETERS

### **Request Parameters (khi tạo URL):**

| Parameter | Mô tả | Ví dụ |
|-----------|-------|-------|
| `vnp_Version` | API version | `2.1.0` |
| `vnp_Command` | Command type | `pay` |
| `vnp_TmnCode` | Terminal code | `DEMO123456` |
| `vnp_Amount` | Số tiền (x100) | `20000000` (200,000 VND) |
| `vnp_CurrCode` | Currency | `VND` |
| `vnp_TxnRef` | Transaction reference | `WLT-1-20250117123456` |
| `vnp_OrderInfo` | Thông tin đơn hàng | `wallet_topup_1` |
| `vnp_OrderType` | Loại đơn hàng | `topup`, `other` |
| `vnp_Locale` | Ngôn ngữ | `vn`, `en` |
| `vnp_CreateDate` | Thời gian tạo | `20250117123456` (GMT+7) |
| `vnp_IpAddr` | IP address | `127.0.0.1` |
| `vnp_ReturnUrl` | Callback URL | `https://yourapp.com/callback` |
| `vnp_BankCode` | Mã ngân hàng (optional) | `NCB`, `VIETCOMBANK` |
| `vnp_SecureHash` | Signature | `abc123...` (tính toán) |

### **Response Parameters (từ Callback):**

| Parameter | Mô tả | Giá trị |
|-----------|-------|---------|
| `vnp_ResponseCode` | Response code | `00` = Success<br>`07` = Trùng lặp<br>`09` = Thẻ/Tài khoản chưa đăng ký<br>`10` = Xác thực không thành công<br>`11` = Đã hết hạn<br>`12` = Thẻ/Tài khoản bị khóa<br>`51` = Không đủ số dư<br>`65` = Tài khoản vượt quá hạn mức<br>`75` = Ngân hàng thanh toán đang bảo trì<br>`99` = Lỗi không xác định |
| `vnp_TransactionNo` | Transaction ID từ VNPay | `12345678` |
| `vnp_BankCode` | Mã ngân hàng | `NCB` |
| `vnp_BankTranNo` | Bank transaction number | `VNP123456` |
| `vnp_PayDate` | Thời gian thanh toán | `20250117123456` |
| `vnp_TransactionStatus` | Transaction status | `00` = Success |
| `vnp_TxnRef` | Transaction reference | `WLT-1-20250117123456` |
| `vnp_SecureHash` | Signature để verify | `abc123...` |

---

## ⚠️ LƯU Ý QUAN TRỌNG

### **1. Amount Format:**
- **Input**: `200000` (VND)
- **VNPay**: `20000000` (x100)
- **Lý do**: VNPay yêu cầu amount không có số thập phân

### **2. Signature Verification:**
- **BẮT BUỘC** verify signature trong callback
- **KHÔNG BAO GIỜ** trust data từ VNPay nếu signature không hợp lệ
- **REJECT** payment nếu signature sai

### **3. Duplicate Handling:**
- VNPay có thể gọi callback **NHIỀU LẦN**
- Backend phải **idempotent** (có thể chạy nhiều lần mà kết quả giống nhau)
- Check `payment.Status` trước khi xử lý

### **4. Timeout & Retry:**
- Callback có thể timeout
- VNPay sẽ retry IPN nhiều lần
- Backend phải xử lý nhanh (< 5 giây)

### **5. TxnRef Format:**
- **MUST BE UNIQUE** cho mỗi payment
- Format: `{PREFIX}-{ID}-{TIMESTAMP}`
- Dùng để match Payment record với Callback

---

## 🎯 FLOW DIAGRAM

```
┌─────────────┐
│   Frontend  │
└──────┬──────┘
       │ 1. POST /api/payments/wallets/{id}/topup
       │    { amount: 200000 }
       ↓
┌─────────────┐
│   Backend   │
│             │
│ • Validate wallet
│ • Create Payment (Pending)
│ • Generate TxnRef: WLT-1-20250117123456
│ • Build VNPay URL
│ • Create HMAC-SHA512 signature
│             │
└──────┬──────┘
       │ 2. Return { url: "https://vnpay.vn/..." }
       ↓
┌─────────────┐
│   Frontend  │
└──────┬──────┘
       │ 3. window.location.href = url
       ↓
┌─────────────┐
│    VNPay    │
│   Gateway   │
│             │
│ • User thanh toán
│ • Xử lý payment
│             │
└──────┬──────┘
       │ 4a. Redirect browser (GET callback)
       │ 4b. Call IPN (POST server-to-server)
       ↓
┌─────────────┐
│   Backend   │
│             │
│ • Verify signature
│ • Find Payment by TxnRef
│ • Check vnp_ResponseCode
│ • Update Payment.Status = "Paid"
│ • Add funds to wallet (+200000)
│ • Create Transaction record
│             │
└──────┬──────┘
       │ 5. Return PaymentServiceDto
       ↓
┌─────────────┐
│   Frontend  │
│             │
│ • Show success message
│ • Refresh wallet balance
│             │
└─────────────┘
```

---

## 📝 CODE EXAMPLES

### **Tạo Payment URL:**

```csharp
// Input
var amount = 200000m;  // 200,000 VND
var walletId = 1;
var userId = 123;

// Process
var txnRef = $"WLT-{walletId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
var vnpAmount = (long)(amount * 100);  // 20000000

var dict = new SortedDictionary<string, string>
{
    ["vnp_Version"] = "2.1.0",
    ["vnp_Command"] = "pay",
    ["vnp_TmnCode"] = "DEMO123456",
    ["vnp_Amount"] = "20000000",
    ["vnp_CurrCode"] = "VND",
    ["vnp_TxnRef"] = txnRef,
    ["vnp_OrderInfo"] = "wallet_topup_1",
    ["vnp_OrderType"] = "topup",
    ["vnp_Locale"] = "vn",
    ["vnp_CreateDate"] = "20250117123456",
    ["vnp_IpAddr"] = "127.0.0.1",
    ["vnp_ReturnUrl"] = "https://yourapp.com/callback"
};

// Signature
var raw = "vnp_Amount=20000000&vnp_Command=pay&...";
var signature = HMACSHA512(hashSecret, raw);  // "abc123def456..."

// URL
var url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?" +
          "vnp_Amount=20000000&vnp_Command=pay&...&vnp_SecureHash=abc123def456...";
```

### **Xử lý Callback:**

```csharp
// Input từ VNPay
var callback = new VnpayCallbackServiceDto
{
    vnp_Amount = "20000000",
    vnp_ResponseCode = "00",  // Success
    vnp_TxnRef = "WLT-1-20250117123456",
    vnp_TransactionNo = "12345678",
    vnp_SecureHash = "abc123def456..."
};

// Verify
var fields = new SortedDictionary<string, string>
{
    ["vnp_Amount"] = callback.vnp_Amount,
    ["vnp_ResponseCode"] = callback.vnp_ResponseCode,
    ["vnp_TxnRef"] = callback.vnp_TxnRef,
    // ... other fields (KHÔNG có vnp_SecureHash)
};

var raw = string.Join("&", fields.Select(kv => $"{kv.Key}={kv.Value}"));
var signed = HMACSHA512(hashSecret, raw);

if (signed == callback.vnp_SecureHash)
{
    // Signature hợp lệ → Xử lý payment
    if (callback.vnp_ResponseCode == "00")
    {
        // Payment thành công
        payment.Status = "Paid";
        
        // Wallet top-up: Nạp tiền
        await _walletService.AddFundsByUserIdAsync(userId, 200000);
    }
}
```

---

## 🧪 TESTING

### **Test với Sandbox:**

1. **Sandbox URL**: `https://sandbox.vnpayment.vn/paymentv2/vpcpay.html`
2. **Test Cards**: VNPay cung cấp test card numbers
3. **ResponseCode = "00"**: Luôn thành công trong sandbox

### **Test Callback:**

```bash
# Simulate VNPay callback
curl "http://localhost:5217/api/payments/vnpay-callback?\
vnp_Amount=20000000&\
vnp_ResponseCode=00&\
vnp_TxnRef=WLT-1-20250117123456&\
vnp_TransactionNo=12345678&\
vnp_SecureHash=..."
```

---

## 📚 TÀI LIỆU THAM KHẢO

- **VNPay API Documentation**: https://sandbox.vnpayment.vn/
- **Backend Code**: `Eduprompt.BLL/Services/PaymentService.cs`
- **Controller**: `Eduprompt.API/Controllers/PaymentsController.cs`
- **Setup Guide**: `Note/VNPAY_SETUP_GUIDE.md`

---

**Updated:** 2025-01-17  
**Version:** 1.0

