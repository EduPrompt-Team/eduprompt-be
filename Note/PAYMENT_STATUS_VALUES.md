# 💳 PAYMENT STATUS VALUES

## ✅ CÓ - Payment Entity có field Status

**File:** `Eduprompt.Domain/Entities/Payment.cs`

```csharp
public partial class Payment
{
    public int PaymentId { get; set; }
    public int? OrderId { get; set; }
    public int? UserId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string Provider { get; set; }
    public string Status { get; set; }  // ← CÓ STATUS
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    // ...
}
```

**Database:**
- Column: `Status NVARCHAR(50) NULL`
- Default Value: `"Pending"`
- Index: `IX_Payments_Status` (để query nhanh)

---

## 📊 TẤT CẢ PAYMENT STATUS VALUES

### **1. "Pending" - Chờ xử lý** ⏳
- **Khi nào:** Khi tạo Payment record lần đầu
- **Mô tả:** Payment đã được tạo, đang chờ user thanh toán trên VNPay
- **Code location:**
  ```csharp
  // PaymentService.cs - Line 111, 429, 496
  Status = "Pending"
  ```

**Flow:**
```
User yêu cầu thanh toán
   ↓
Backend tạo Payment với Status = "Pending"
   ↓
Backend tạo VNPay URL
   ↓
User redirect đến VNPay
   ↓
Status vẫn = "Pending" (chờ callback)
```

---

### **2. "Paid" - Đã thanh toán** ✅
- **Khi nào:** Khi VNPay callback thành công (`vnp_ResponseCode = "00"`)
- **Mô tả:** Payment đã được thanh toán thành công
- **Code location:**
  ```csharp
  // PaymentService.cs - Line 159, 355
  var success = cb.vnp_ResponseCode == "00";
  payment.Status = success ? "Paid" : "Failed";
  ```

**Flow:**
```
VNPay callback với vnp_ResponseCode = "00"
   ↓
Backend verify signature thành công
   ↓
Backend update Payment.Status = "Paid"
   ↓
Backend xử lý business logic:
   - Wallet top-up: Nạp tiền vào wallet
   - Order payment: Update Order.Status = "Paid"
   - Transaction payment: Tạo Transaction record
```

---

### **3. "Failed" - Thanh toán thất bại** ❌
- **Khi nào:**
  1. Signature không hợp lệ (bị giả mạo)
  2. `vnp_ResponseCode != "00"` (user hủy thanh toán, không đủ tiền, ...)
- **Mô tả:** Payment không thành công
- **Code location:**
  ```csharp
  // PaymentService.cs - Line 150, 159
  if (signed != secure)
  {
      payment.Status = "Failed";  // Signature không hợp lệ
  }
  else
  {
      var success = cb.vnp_ResponseCode == "00";
      payment.Status = success ? "Paid" : "Failed";  // ResponseCode != "00"
  }
  ```

**VNPay Response Codes (khác "00" = Failed):**
- `07`: Trùng lặp
- `09`: Thẻ/Tài khoản chưa đăng ký
- `10`: Xác thực không thành công
- `11`: Đã hết hạn
- `12`: Thẻ/Tài khoản bị khóa
- `51`: Không đủ số dư
- `65`: Tài khoản vượt quá hạn mức
- `75`: Ngân hàng thanh toán đang bảo trì
- `99`: Lỗi không xác định

---

### **4. "Refunded" - Đã hoàn tiền** 🔄
- **Khi nào:** Khi admin thực hiện refund qua VNPay
- **Mô tả:** Payment đã được hoàn tiền
- **Code location:**
  ```csharp
  // PaymentsController.cs - Line 71-77
  [HttpPost("refund")]
  [Authorize(Policy = "AdminOnly")]
  public async Task<IActionResult> Refund([FromBody] VnpayRefundRequestDto dto)
  {
      var result = await _paymentService.RefundVnpayTransactionAsync(dto);
      // Cần update status = "Refunded" sau khi refund thành công
  }
  ```

**Lưu ý:** Hiện tại code chưa tự động update status = "Refunded" sau refund. Cần thêm logic này.

---

### **5. "Cancelled" - Đã hủy** 🚫
- **Khi nào:** Khi admin/user hủy payment
- **Mô tả:** Payment đã bị hủy
- **Code location:**
  ```csharp
  // PaymentsController.cs - Line 104-109
  [HttpPatch("{paymentId}/status")]
  [Authorize(Policy = "AdminOnly")]
  public async Task<IActionResult> UpdateStatus(int paymentId, [FromQuery] string status)
  {
      var result = await _paymentService.UpdatePaymentStatusAsync(paymentId, status);
      // Admin có thể set status = "Cancelled"
  }
  ```

---

## 🔄 STATUS FLOW

```
┌──────────┐
│ Pending  │ ← Tạo payment lần đầu
└────┬─────┘
     │
     ├─────────────────┐
     │                 │
     ↓                 ↓
┌─────────┐      ┌─────────┐
│  Paid   │      │ Failed  │
└────┬────┘      └─────────┘
     │
     ↓
┌──────────┐
│ Refunded │ ← Admin refund (nếu cần)
└──────────┘

Hoặc:

┌──────────┐
│ Pending  │
└────┬─────┘
     │
     ↓ (Admin cancel)
┌───────────┐
│ Cancelled │
└───────────┘
```

---

## 📝 CODE EXAMPLES

### **Tạo Payment với Status = "Pending":**

```csharp
// PaymentService.cs - CreateVnpayUrlForWalletTopupAsync
var payment = new Payment
{
    OrderId = null,
    UserId = userId,
    Amount = amount,
    PaymentMethod = "Online",
    Provider = "VNPay",
    Status = "Pending",  // ← Default status
    CreatedAt = DateTime.UtcNow,
    TxnRef = txnRef
};
await _paymentRepository.CreateAsync(payment);
```

### **Update Status sau Callback:**

```csharp
// PaymentService.cs - ProcessVnpayCallbackAsync
var success = cb.vnp_ResponseCode == "00";
payment.Status = success ? "Paid" : "Failed";  // ← Update status
payment.UpdatedAt = DateTime.UtcNow;
await _paymentRepository.UpdateAsync(payment);
```

### **Admin update Status thủ công:**

```csharp
// PaymentService.cs - UpdatePaymentStatusAsync
public async Task<PaymentServiceDto> UpdatePaymentStatusAsync(int paymentId, string status)
{
    var payment = await _paymentRepository.GetByIdAsync(paymentId);
    payment.Status = status;  // ← Admin có thể set bất kỳ status nào
    payment.UpdatedAt = DateTime.UtcNow;
    await _paymentRepository.UpdateAsync(payment);
    return Map(payment);
}
```

---

## 📊 STATUS TRONG DATABASE

### **Default Value:**
```sql
Status NVARCHAR(50) NULL DEFAULT 'Pending'
```

### **Index:**
```sql
CREATE INDEX IX_Payments_Status ON Payments(Status);
```

### **Query examples:**
```sql
-- Tìm tất cả payments đang pending
SELECT * FROM Payments WHERE Status = 'Pending';

-- Tìm payments đã thanh toán
SELECT * FROM Payments WHERE Status = 'Paid';

-- Tìm payments thất bại
SELECT * FROM Payments WHERE Status = 'Failed';

-- Thống kê theo status
SELECT Status, COUNT(*) as Count 
FROM Payments 
GROUP BY Status;
```

---

## 🎯 STATUS VALUES SUMMARY

| Status | Mô tả | Khi nào | Có thể set? |
|--------|-------|---------|-------------|
| `"Pending"` | Chờ xử lý | Tạo payment lần đầu | ✅ Tự động |
| `"Paid"` | Đã thanh toán | VNPay callback thành công | ✅ Tự động |
| `"Failed"` | Thất bại | Signature sai hoặc ResponseCode != "00" | ✅ Tự động |
| `"Refunded"` | Đã hoàn tiền | Admin refund | ⚠️ Cần thêm logic |
| `"Cancelled"` | Đã hủy | Admin/user hủy | ✅ Admin manual |

---

## ✅ KẾT LUẬN

**Payment CÓ Status field:**
- ✅ Type: `string`
- ✅ Database: `NVARCHAR(50) NULL`
- ✅ Default: `"Pending"`
- ✅ Index: `IX_Payments_Status`
- ✅ Các giá trị: `"Pending"`, `"Paid"`, `"Failed"`, `"Refunded"`, `"Cancelled"`

**Status được sử dụng để:**
- Track payment state
- Query payments theo trạng thái
- Business logic routing (chỉ xử lý khi Paid)
- Reporting và analytics

---

**Updated:** 2025-01-17

