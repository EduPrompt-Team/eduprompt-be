# 🧪 HƯỚNG DẪN TEST VNPAY VỚI LOCALHOST

## 📋 CẤU HÌNH HIỆN TẠI

```json
{
  "VNPay": {
    "Url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "TmnCode": "BPK6D86O",
    "HashSecret": "IXS344BB3EA9QG311VUUAD3JAF95JVMD",
    "ReturnUrl": "https://localhost:7199/api/payments/vnpay-callback",
    "IpnUrl": "https://localhost:7199/api/payments/vnpay-ipn"
  }
}
```

---

## ✅ CÓ THỂ TEST ĐƯỢC

### **1. Luồng Tạo Payment URL (Chuyển khoản) ✅**

```
Frontend → Backend: POST /api/payments/wallets/1/topup
   ↓
Backend tạo Payment URL với ReturnUrl = localhost
   ↓
Backend trả về URL cho Frontend
   ↓
Frontend redirect user đến VNPay
   ↓
User thanh toán trên VNPay
```

**Kết quả:** ✅ **HOẠT ĐỘNG BÌNH THƯỜNG**

**Lý do:**
- Backend chỉ cần tạo URL, không cần VNPay gọi về
- User browser có thể truy cập localhost
- VNPay sandbox có thể redirect về localhost (browser redirect)

---

### **2. Luồng Callback (Browser Redirect) ⚠️ MỘT PHẦN**

```
VNPay → Browser: Redirect về localhost:7199/api/payments/vnpay-callback
   ↓
Browser: User đang ở localhost → Có thể nhận được callback
   ↓
Backend xử lý callback
```

**Kết quả:** ⚠️ **CHỈ HOẠT ĐỘNG NẾU USER ĐANG Ở LOCALHOST**

**Điều kiện:**
- ✅ User phải đang truy cập từ `localhost` hoặc `127.0.0.1`
- ✅ Backend phải đang chạy trên `localhost:7199`
- ✅ Browser của user có thể truy cập localhost

**Khi nào KHÔNG hoạt động:**
- ❌ User truy cập từ thiết bị khác (mobile, máy khác)
- ❌ VNPay redirect nhưng browser không thể kết nối localhost

---

## ❌ KHÔNG THỂ TEST ĐƯỢC

### **3. Luồng IPN (Server-to-Server) ❌**

```
VNPay Server → Backend Server: POST /api/payments/vnpay-ipn
   ↓
VNPay KHÔNG THỂ gọi localhost từ server của họ
   ↓
IPN sẽ FAIL
```

**Kết quả:** ❌ **KHÔNG HOẠT ĐỘNG**

**Lý do:**
- VNPay server không thể truy cập `localhost:7199` từ server của họ
- `localhost` chỉ có thể truy cập từ chính máy đó
- IPN cần public URL để VNPay server có thể gọi

**Ảnh hưởng:**
- ⚠️ Backend vẫn có thể xử lý qua Callback (browser redirect)
- ⚠️ Nhưng IPN là backup mechanism, nên thiếu IPN = thiếu reliability

---

## 📊 BẢNG ĐÁNH GIÁ

| Luồng | Có thể test? | Ghi chú |
|-------|-------------|---------|
| **Tạo Payment URL** | ✅ CÓ | Hoạt động bình thường |
| **User thanh toán trên VNPay** | ✅ CÓ | Hoạt động bình thường |
| **Callback (Browser redirect)** | ⚠️ MỘT PHẦN | Chỉ hoạt động nếu user ở localhost |
| **IPN (Server-to-server)** | ❌ KHÔNG | VNPay không thể gọi localhost |
| **Xử lý business logic** | ⚠️ MỘT PHẦN | Phụ thuộc vào callback có hoạt động không |

---

## 🔧 GIẢI PHÁP ĐỂ TEST ĐẦY ĐỦ

### **GIẢI PHÁP 1: Dùng Ngrok (Khuyến nghị cho Development) ⭐**

**Ngrok** tạo public URL → tunnel về localhost:

```bash
# 1. Cài đặt ngrok (nếu chưa có)
# Download: https://ngrok.com/download

# 2. Chạy ngrok để expose localhost:7199
ngrok http 7199

# Output:
# Forwarding: https://abc123.ngrok.io -> http://localhost:7199
```

**Cập nhật appsettings.json:**

```json
{
  "VNPay": {
    "Url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "TmnCode": "BPK6D86O",
    "HashSecret": "IXS344BB3EA9QG311VUUAD3JAF95JVMD",
    "ReturnUrl": "https://abc123.ngrok.io/api/payments/vnpay-callback",
    "IpnUrl": "https://abc123.ngrok.io/api/payments/vnpay-ipn"
  }
}
```

**Lưu ý:**
- ✅ Free tier của ngrok có giới hạn
- ✅ URL thay đổi mỗi lần restart ngrok (trừ khi dùng paid plan)
- ✅ Cần cấu hình URL mới trong VNPay dashboard mỗi lần

---

### **GIẢI PHÁP 2: Deploy lên Server Test**

Deploy backend lên server có public IP/domain:

```json
{
  "VNPay": {
    "ReturnUrl": "https://test-api.yourapp.com/api/payments/vnpay-callback",
    "IpnUrl": "https://test-api.yourapp.com/api/payments/vnpay-ipn"
  }
}
```

**Lợi ích:**
- ✅ URL cố định
- ✅ Test được cả Callback và IPN
- ✅ Giống production environment

---

### **GIẢI PHÁP 3: Test thủ công Callback (Development)**

Nếu chỉ test logic xử lý callback, có thể simulate thủ công:

```bash
# Simulate VNPay callback
curl "http://localhost:5217/api/payments/vnpay-callback?\
vnp_Amount=20000000&\
vnp_BankCode=NCB&\
vnp_TransactionNo=12345678&\
vnp_ResponseCode=00&\
vnp_TxnRef=WLT-1-20250117123456&\
vnp_SecureHash=..."
```

**Hoặc dùng Postman/Thunder Client** để gọi callback endpoint trực tiếp.

---

## 🎯 KỊCH BẢN TEST VỚI LOCALHOST

### **Kịch bản 1: Test đầy đủ (Dùng ngrok)**

```
1. ✅ Start backend: dotnet run (port 7199)
2. ✅ Start ngrok: ngrok http 7199
3. ✅ Lấy public URL từ ngrok: https://abc123.ngrok.io
4. ✅ Update appsettings.json với ngrok URL
5. ✅ Restart backend
6. ✅ Frontend gọi: POST /api/payments/wallets/1/topup
7. ✅ Redirect user đến VNPay URL
8. ✅ User thanh toán trên VNPay
9. ✅ VNPay redirect về ngrok URL → Backend nhận callback ✅
10. ✅ VNPay gọi IPN → ngrok URL → Backend nhận IPN ✅
```

**Kết quả:** ✅ **TEST ĐẦY ĐỦ CẢ 2 LUỒNG**

---

### **Kịch bản 2: Test một phần (Không dùng ngrok)**

```
1. ✅ Start backend: dotnet run (port 7199)
2. ✅ Frontend gọi: POST /api/payments/wallets/1/topup
3. ✅ Redirect user đến VNPay URL
4. ✅ User thanh toán trên VNPay
5. ⚠️ VNPay redirect về localhost → Chỉ hoạt động nếu user ở localhost
6. ❌ VNPay gọi IPN → localhost → FAIL (VNPay không thể gọi)
```

**Kết quả:** ⚠️ **TEST MỘT PHẦN** (Callback có thể hoạt động, IPN không)

---

### **Kịch bản 3: Test logic thủ công**

```
1. ✅ Start backend
2. ✅ Tạo Payment record thủ công (hoặc qua API)
3. ✅ Simulate callback bằng curl/Postman
4. ✅ Verify business logic xử lý đúng
```

**Kết quả:** ✅ **TEST LOGIC** (Không test flow thực tế)

---

## 📝 CHECKLIST ĐỂ TEST

### **Với localhost (không ngrok):**
- [ ] ✅ Tạo Payment URL - **Hoạt động**
- [ ] ✅ User thanh toán trên VNPay - **Hoạt động**
- [ ] ⚠️ Callback - **Chỉ nếu user ở localhost**
- [ ] ❌ IPN - **KHÔNG hoạt động**

### **Với ngrok:**
- [ ] ✅ Tạo Payment URL - **Hoạt động**
- [ ] ✅ User thanh toán trên VNPay - **Hoạt động**
- [ ] ✅ Callback - **Hoạt động**
- [ ] ✅ IPN - **Hoạt động**

---

## 🔍 CÁCH KIỂM TRA CALLBACK CÓ HOẠT ĐỘNG

### **Test 1: Kiểm tra Backend có nhận được callback**

```csharp
// Thêm logging trong PaymentsController.cs
[HttpGet("vnpay-callback")]
[AllowAnonymous]
public async Task<IActionResult> VnpayCallback([FromQuery] VnpayCallbackServiceDto cb)
{
    // Log để kiểm tra
    _logger.LogInformation("VNPay Callback received: {TxnRef}, {ResponseCode}", 
        cb.vnp_TxnRef, cb.vnp_ResponseCode);
    
    var result = await _paymentService.ProcessVnpayCallbackAsync(cb);
    return Ok(result);
}
```

### **Test 2: Kiểm tra Payment status sau callback**

```bash
# Sau khi thanh toán, check payment status
GET /api/payments/{paymentId}

# Expected: Status = "Paid" nếu thành công
```

---

## ⚠️ LƯU Ý QUAN TRỌNG

### **1. HTTPS vs HTTP:**
- VNPay yêu cầu `ReturnUrl` phải là HTTPS (production)
- Sandbox có thể chấp nhận HTTP
- Ngrok tự động cung cấp HTTPS

### **2. Certificate:**
- Localhost HTTPS cần certificate hợp lệ
- Ngrok tự động handle certificate

### **3. CORS:**
- Đảm bảo CORS cho phép requests từ VNPay domain

---

## 🎯 KẾT LUẬN

**Với cấu hình hiện tại (`localhost:7199`):**

✅ **CÓ THỂ TEST:**
- Luồng tạo Payment URL
- User thanh toán trên VNPay
- Callback (nếu user ở localhost)

❌ **KHÔNG THỂ TEST:**
- IPN (server-to-server)
- Callback từ thiết bị khác

**Khuyến nghị:**
- **Development:** Dùng **ngrok** để test đầy đủ
- **Production:** Deploy lên server có public URL

---

**Updated:** 2025-01-17

