# VNPay Topup 500 Error - Fix Summary

## ✅ ĐÃ SỬA

### 1. **PaymentsController.cs**
- ✅ Thêm try-catch đầy đủ cho `CreateVnpayUrlForWalletTopup`
- ✅ Thêm validation cho request body và amount
- ✅ Thêm logging chi tiết để debug (Console.WriteLine + Debug.WriteLine)
- ✅ Trả về error message chi tiết với `type` và `stackTrace`

### 2. **PaymentService.cs**
- ✅ Thêm validation cho VNPay config (Url, TmnCode, HashSecret, ReturnUrl)
- ✅ Thêm try-catch khi tạo Payment record (không throw exception, vẫn return URL)
- ✅ Cho phép VNPay flow tiếp tục ngay cả khi payment record creation fail

---

## 🔴 CẦN LÀM NGAY

### **RESTART BACKEND**

Backend **PHẢI** được restart để code mới có hiệu lực:

```powershell
# 1. Dừng backend hiện tại
# - Nhấn Ctrl+C trong terminal đang chạy backend
# - Hoặc kill process:
Get-Process -Name "Eduprompt.API" | Stop-Process -Force

# 2. Build lại (optional, nhưng recommended)
cd E:\eduprompt-be\Eduprompt.API
dotnet build

# 3. Chạy lại
dotnet run
```

---

## 🔍 DEBUG

Sau khi restart backend, nếu vẫn lỗi 500:

### 1. **Kiểm tra Backend Console Logs**
Backend sẽ log chi tiết:
```
VNPay Topup Error: [Exception details]
```

### 2. **Kiểm tra Frontend Error Response**
Frontend sẽ nhận được:
```json
{
  "message": "Internal server error: [chi tiết]",
  "type": "ExceptionType",
  "stackTrace": "..."
}
```

### 3. **Các Nguyên Nhân Có Thể**

#### a) **VNPay Config Thiếu**
- Kiểm tra `appsettings.json`:
  ```json
  "VNPay": {
    "Url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "TmnCode": "BPK6D86O",
    "HashSecret": "IXS344BB3EA9QG311VUUAD3JAF95JVMD",
    "ReturnUrl": "https://localhost:7199/api/payments/vnpay-callback"
  }
  ```

#### b) **Wallet Không Tồn Tại**
- Wallet ID = 12 phải tồn tại trong database
- Wallet phải thuộc về user hiện tại (userId từ JWT token)

#### c) **Database Error**
- Kiểm tra Payments table có đúng schema không
- Kiểm tra connection string trong `appsettings.json`

#### d) **UserId Null**
- UserId từ JWT token phải hợp lệ (int)
- Kiểm tra JWT token có đúng format không

---

## 📋 CHECKLIST

- [x] Sửa PaymentsController - thêm try-catch và logging
- [x] Sửa PaymentService - thêm validation và error handling
- [ ] **CẦN LÀM**: Restart backend
- [ ] **CẦN LÀM**: Test lại endpoint
- [ ] **CẦN LÀM**: Kiểm tra backend logs nếu vẫn lỗi

---

## 🎯 KẾT QUẢ MONG ĐỢI

Sau khi restart backend:
1. ✅ Endpoint trả về VNPay URL thành công (200 OK)
2. ✅ Hoặc trả về error message chi tiết (400/404/403/500) với thông tin debug
3. ✅ Backend console sẽ log chi tiết nếu có exception

---

## ⚠️ LƯU Ý

1. **Backend PHẢI restart** - code mới chưa được load nếu chưa restart
2. **Kiểm tra backend logs** - sẽ có thông tin chi tiết về exception
3. **Frontend error message** - sẽ hiển thị chi tiết hơn sau khi restart

---

**Sau khi restart backend, test lại và xem backend console logs để biết lỗi cụ thể!** 🔍

