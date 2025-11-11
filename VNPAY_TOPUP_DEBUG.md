# VNPay Topup 500 Error - Debug Guide

## 🔴 VẤN ĐỀ
Frontend gọi `POST /api/payments/wallets/12/topup` và nhận lỗi **500 Internal Server Error**.

## ✅ ĐÃ SỬA

### 1. **PaymentsController.cs**
- ✅ Thêm try-catch với error handling chi tiết
- ✅ Validation cho request body và amount
- ✅ Trả về error message rõ ràng thay vì 500 generic

### 2. **PaymentService.cs**
- ✅ Validation cho VNPay config (Url, TmnCode, HashSecret, ReturnUrl)
- ✅ Ném exception rõ ràng nếu config thiếu

---

## 🚨 CẦN LÀM NGAY

### **BƯỚC 1: Restart Backend**

Backend **PHẢI** được restart để áp dụng code mới:

```powershell
# 1. Dừng backend hiện tại (Ctrl+C trong terminal đang chạy)
# Hoặc kill process:
Get-Process -Name "Eduprompt.API" | Stop-Process -Force

# 2. Build lại (optional, nhưng nên làm):
cd E:\eduprompt-be\Eduprompt.API
dotnet clean
dotnet build

# 3. Chạy lại:
dotnet run
```

---

## 🔍 DEBUG STEPS

### **BƯỚC 2: Kiểm Tra Backend Logs**

Sau khi restart, khi frontend gọi API, xem console output của backend:

**Nếu thấy lỗi:**
- `"VNPay URL is not configured"` → Kiểm tra `appsettings.json` → `VNPay:Url`
- `"VNPay TmnCode is not configured"` → Kiểm tra `appsettings.json` → `VNPay:TmnCode`
- `"VNPay HashSecret is not configured"` → Kiểm tra `appsettings.json` → `VNPay:HashSecret`
- `"VNPay ReturnUrl is not configured"` → Kiểm tra `appsettings.json` → `VNPay:ReturnUrl`
- `"Wallet not found"` → Wallet ID = 12 không tồn tại trong DB
- `"Wallet does not belong to user"` → Wallet ID = 12 không thuộc về user hiện tại
- `"Amount must be greater than 0"` → Amount <= 0

**Nếu thấy lỗi database:**
- Có thể là constraint violation hoặc null reference
- Kiểm tra Payment table schema
- Kiểm tra UserId có tồn tại không

---

## 📋 KIỂM TRA CONFIG

### **BƯỚC 3: Kiểm Tra appsettings.json**

File: `E:\eduprompt-be\Eduprompt.API\appsettings.json`

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

**Đảm bảo:**
- ✅ Tất cả fields đều có giá trị (không rỗng)
- ✅ `ReturnUrl` phải là URL hợp lệ
- ✅ `HashSecret` phải đúng với VNPay sandbox account

---

## 🧪 TEST MANUAL

### **BƯỚC 4: Test Trực Tiếp Với Swagger**

1. Mở Swagger: `https://localhost:7199/swagger` hoặc `http://localhost:5217/swagger`
2. Tìm endpoint: `POST /api/payments/wallets/{walletId}/topup`
3. Click "Try it out"
4. Nhập:
   - `walletId`: 12
   - Body:
     ```json
     {
       "amount": 100000,
       "language": "vn",
       "returnUrl": "http://localhost:5173/wallet/topup"
     }
     ```
5. Click "Execute"
6. Xem response:
   - **200 OK** → Thành công, có `url` trong response
   - **400/404/403** → Xem `message` để biết lỗi cụ thể
   - **500** → Xem backend console logs để biết exception

---

## 🐛 COMMON ISSUES

### **Issue 1: Backend Chưa Restart**
**Symptom:** Vẫn lỗi 500 với message generic
**Fix:** Restart backend (xem BƯỚC 1)

### **Issue 2: VNPay Config Thiếu**
**Symptom:** Error message: "VNPay XXX is not configured"
**Fix:** Kiểm tra `appsettings.json` (xem BƯỚC 3)

### **Issue 3: Wallet Không Tồn Tại**
**Symptom:** Error message: "Wallet not found"
**Fix:** 
- Kiểm tra wallet ID = 12 có tồn tại trong DB không
- Hoặc dùng wallet ID khác (lấy từ `/api/wallets/my-wallet`)

### **Issue 4: Wallet Không Thuộc Về User**
**Symptom:** Error message: "Wallet does not belong to user"
**Fix:** 
- Đảm bảo user đang login đúng
- Hoặc wallet ID = 12 phải thuộc về user hiện tại

### **Issue 5: Database Constraint Violation**
**Symptom:** Error message: "Internal server error: ..." với SQL error
**Fix:**
- Kiểm tra Payment table có đủ columns không
- Kiểm tra UserId có tồn tại trong Users table không
- Kiểm tra foreign key constraints

---

## 📝 EXPECTED RESPONSE

### **Success Response (200 OK):**
```json
{
  "url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_Amount=10000000&vnp_Command=pay&..."
}
```

### **Error Response (400/404/403):**
```json
{
  "message": "Wallet not found"
}
```

### **Error Response (500):**
```json
{
  "message": "Internal server error: VNPay HashSecret is not configured",
  "details": "System.InvalidOperationException: VNPay HashSecret is not configured\n   at ..."
}
```

---

## ✅ CHECKLIST

- [ ] Backend đã được restart sau khi sửa code
- [ ] VNPay config trong `appsettings.json` đầy đủ
- [ ] Wallet ID = 12 tồn tại trong database
- [ ] Wallet ID = 12 thuộc về user hiện tại
- [ ] Test với Swagger để xem error message cụ thể
- [ ] Kiểm tra backend console logs khi gọi API

---

## 🚀 NEXT STEPS

1. **Restart backend** (BƯỚC 1)
2. **Test với Swagger** (BƯỚC 4)
3. **Xem error message cụ thể** từ response
4. **Fix theo error message** (xem COMMON ISSUES)

---

**Sau khi restart backend, test lại và cho tôi biết error message cụ thể!** 🎯

