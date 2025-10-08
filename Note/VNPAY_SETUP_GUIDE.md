# 💳 VNP AY PAYMENT INTEGRATION GUIDE

## 📋 Tổng quan

Hệ thống Eduprompt đã được tích hợp sẵn **VNPAY Payment Gateway** để xử lý thanh toán trực tuyến. Tất cả code đã được implement, bạn chỉ cần cấu hình credentials từ VNPAY.

---

## 🚀 Payment Flow

```
1. User tạo Order (POST /api/orders)
   ↓
2. User chọn thanh toán VNPAY (POST /api/payments/orders/{orderId}/vnpay)
   ↓
3. System tạo VNPAY Payment URL
   ↓
4. User được redirect tới VNPAY để thanh toán
   ↓
5. User hoàn tất thanh toán trên VNPAY
   ↓
6. VNPAY gọi callback (GET /api/payments/vnpay-callback)
   ↓
7. System cập nhật Payment + Order status
   ↓
8. Nếu thành công → Thêm templates vào User's Storage
```

---

## ⚙️ CẤU HÌNH VNPAY

### **Bước 1: Đăng ký tài khoản VNPAY**

1. Truy cập: https://sandbox.vnpayment.vn/
2. Đăng ký tài khoản merchant (môi trường sandbox để test)
3. Sau khi đăng ký, bạn sẽ nhận được:
   - **TMN Code** (Mã định danh merchant)
   - **Hash Secret** (Key bí mật để mã hóa)

### **Bước 2: Cập nhật appsettings.json**

Mở file `Eduprompt.API/appsettings.json` và cập nhật:

```json
{
  "VNPay": {
    "Url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "TmnCode": "YOUR_TMN_CODE_HERE",           // ← Thay bằng TMN Code từ VNPAY
    "HashSecret": "YOUR_HASH_SECRET_HERE",     // ← Thay bằng Hash Secret từ VNPAY
    "ReturnUrl": "https://yourwebsite.com/api/payments/vnpay-callback"  // ← URL callback của bạn
  }
}
```

**Ví dụ sau khi cập nhật:**
```json
{
  "VNPay": {
    "Url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "TmnCode": "DEMO123456",
    "HashSecret": "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456",
    "ReturnUrl": "https://eduprompt.com/api/payments/vnpay-callback"
  }
}
```

### **Bước 3: Cấu hình Return URL trong VNPAY Dashboard**

1. Đăng nhập vào VNPAY merchant portal
2. Vào **Cấu hình → IPN URL**
3. Thêm URL: `https://yourwebsite.com/api/payments/vnpay-callback`
4. **Lưu ý:** URL này phải publicly accessible (VNPAY sẽ gọi callback)

---

## 🧪 TEST PAYMENT FLOW

### **1. Tạo Order**
```http
POST /api/orders
Authorization: Bearer {token}
{
  "notes": "Test order"
}
```

Response:
```json
{
  "orderId": 1,
  "orderNumber": "ORD20230101001",
  "totalAmount": 100000,
  "status": "Pending"
}
```

### **2. Tạo VNPAY Payment URL**
```http
POST /api/payments/orders/1/vnpay
Authorization: Bearer {token}
{
  "paymentMethod": "VNPAY",
  "bankCode": "NCB",
  "language": "vn"
}
```

Response:
```json
{
  "success": true,
  "paymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_Amount=10000000&...",
  "message": "Redirect user to this URL to complete payment"
}
```

### **3. Redirect User**
- Frontend redirect user tới `paymentUrl`
- User thanh toán trên VNPAY
- VNPAY sẽ callback về `/api/payments/vnpay-callback`

### **4. Verify Payment**
```http
GET /api/payments/orders/1
Authorization: Bearer {token}
```

Response:
```json
[
  {
    "paymentId": 1,
    "orderId": 1,
    "paymentMethod": "VNPAY_NCB_123456",
    "amount": 100000,
    "status": "Completed"
  }
]
```

---

## 📝 BANK CODES (Sandbox Testing)

VNPAY Sandbox hỗ trợ các mã ngân hàng test:

| Bank Code | Ngân hàng | Số thẻ test | Tên test | Ngày hết hạn | CVV |
|-----------|-----------|-------------|----------|--------------|-----|
| NCB | NCB Bank | 9704198526191432198 | NGUYEN VAN A | 07/15 | 123 |
| VIETCOMBANK | Vietcombank | 9704061704190732197 | NGUYEN VAN B | 03/07 | 532 |
| AGRIBANK | Agribank | 9704062001109320000 | NGUYEN VAN C | 03/07 | 678 |

**OTP test:** `123456`

---

## 🔐 SECURITY NOTES

1. **Hash Secret** phải được bảo mật tuyệt đối
2. Không commit Hash Secret vào Git (sử dụng environment variables trong production)
3. Luôn validate signature từ VNPAY callback
4. Sử dụng HTTPS cho Return URL trong production

---

## 🛠️ TROUBLESHOOTING

### **Lỗi: Invalid Signature**
- Kiểm tra lại `HashSecret` trong appsettings.json
- Đảm bảo không có khoảng trắng thừa

### **Lỗi: Invalid TmnCode**
- Kiểm tra lại `TmnCode` từ VNPAY
- Đảm bảo sử dụng đúng môi trường (sandbox/production)

### **Callback không hoạt động**
- Kiểm tra Return URL đã được cấu hình trong VNPAY dashboard
- Đảm bảo endpoint `/api/payments/vnpay-callback` là publicly accessible
- Kiểm tra logs để xem VNPAY có gọi callback không

---

## 📚 API ENDPOINTS SUMMARY

| Endpoint | Method | Auth | Description |
|----------|--------|------|-------------|
| `/api/payments/orders/{orderId}/vnpay` | POST | ✅ | Tạo VNPAY payment URL |
| `/api/payments/vnpay-callback` | GET | ❌ | VNPAY callback endpoint |
| `/api/payments/orders/{orderId}` | GET | ✅ | Get payments by order |
| `/api/payments/admin/all` | GET | 🔑 Admin | Get all payments |
| `/api/payments/admin/{id}/status` | PUT | 🔑 Admin | Update payment status |

---

## 🎯 PRODUCTION CHECKLIST

- [ ] Đăng ký VNPAY production account
- [ ] Cập nhật `TmnCode` và `HashSecret` production
- [ ] Thay `Url` thành production URL của VNPAY
- [ ] Cấu hình Return URL production trong VNPAY dashboard
- [ ] Test payment flow hoàn chỉnh
- [ ] Setup monitoring cho payment failures
- [ ] Backup Hash Secret an toàn

---

## 📞 SUPPORT

- **VNPAY Support:** support@vnpay.vn
- **VNPAY Docs:** https://sandbox.vnpayment.vn/apis/docs/
- **Eduprompt Team:** admin@eduprompt.com 