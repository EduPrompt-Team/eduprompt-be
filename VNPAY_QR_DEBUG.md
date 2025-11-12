# VNPay QR Code Debug Guide

## ✅ ĐÃ SỬA

### 1. **Frontend (PaymentPrompt.tsx)**
- ✅ Hàm `handleVnpayPayment` giờ nhận tham số `bankCode?: string`
- ✅ Truyền `bankCode` vào `paymentService.topupWalletWithVnpay()`
- ✅ Thêm console.log để debug: `VNPay URL created with bankCode: ...`

### 2. **Backend (PaymentService.cs)**
- ✅ `vnp_BankCode` được thêm vào `SortedDictionary` TRƯỚC khi tính hash
- ✅ `SortedDictionary` tự động sắp xếp A-Z (đúng vị trí cho hash)
- ✅ Signature được tính từ query string có `vnp_BankCode` (nếu có)

---

## 🔍 DEBUG STEPS

### 1. **Kiểm tra Frontend Console**
Khi nhấn nút "Thanh toán bằng QR Code", mở Console (F12) và xem:
```
VNPay URL created with bankCode: VNPAYQR
```

Nếu không thấy log này hoặc `bankCode: undefined`, có nghĩa là frontend chưa truyền đúng.

### 2. **Kiểm tra Network Request**
Mở Network tab (F12 → Network), tìm request `POST /api/payments/wallets/{walletId}/topup`:
- Xem Request Payload có `bankCode: "VNPAYQR"` không
- Xem Response có URL không

### 3. **Kiểm tra Backend Logs**
Backend sẽ log:
```
VNPay Topup Error: [nếu có lỗi]
```

Hoặc kiểm tra URL được tạo có chứa `vnp_BankCode=VNPAYQR` không.

### 4. **Kiểm tra VNPay URL**
URL được tạo phải có dạng:
```
https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_Amount=...&vnp_BankCode=VNPAYQR&vnp_Command=pay&...&vnp_SecureHash=...
```

**Quan trọng**: `vnp_BankCode=VNPAYQR` phải có trong URL và phải được tính vào hash.

---

## ⚠️ CÁC VẤN ĐỀ CÓ THỂ XẢY RA

### 1. **VNPay Sandbox không hỗ trợ QR**
- VNPay Sandbox có thể không hiển thị QR code trong môi trường test
- Trong production sẽ hoạt động bình thường
- **Giải pháp**: Test với production VNPay hoặc chấp nhận rằng sandbox có giới hạn

### 2. **Backend chưa restart**
- Code mới chưa được load
- **Giải pháp**: Restart backend

### 3. **Frontend chưa refresh**
- Code mới chưa được load
- **Giải pháp**: Hard refresh (Ctrl+Shift+R) hoặc clear cache

### 4. **bankCode không được truyền**
- Frontend không truyền `bankCode` vào request
- **Giải pháp**: Kiểm tra console log và network request

### 5. **Signature sai**
- `vnp_BankCode` không được tính vào hash
- **Giải pháp**: Đảm bảo `vnp_BankCode` được thêm vào `SortedDictionary` TRƯỚC khi tính hash

---

## 🧪 TEST STEPS

1. **Mở Frontend Console** (F12)
2. **Nhấn nút "Thanh toán bằng QR Code"**
3. **Kiểm tra Console log**: `VNPay URL created with bankCode: VNPAYQR`
4. **Kiểm tra Network tab**: Request có `bankCode: "VNPAYQR"` không
5. **Kiểm tra URL được redirect**: Có `vnp_BankCode=VNPAYQR` không
6. **Kiểm tra VNPay page**: Có hiển thị QR code không

---

## 📋 CHECKLIST

- [x] Frontend truyền `bankCode` vào `handleVnpayPayment('VNPAYQR')`
- [x] Frontend truyền `bankCode` vào `paymentService.topupWalletWithVnpay()`
- [x] Backend nhận `BankCode` từ `WalletTopupRequestDto`
- [x] Backend thêm `vnp_BankCode` vào `SortedDictionary` trước khi tính hash
- [x] Backend tính signature từ query string có `vnp_BankCode`
- [ ] **CẦN TEST**: Frontend console log
- [ ] **CẦN TEST**: Network request payload
- [ ] **CẦN TEST**: VNPay URL có `vnp_BankCode=VNPAYQR`
- [ ] **CẦN TEST**: VNPay page hiển thị QR code

---

## 🎯 KẾT QUẢ MONG ĐỢI

Khi nhấn "Thanh toán bằng QR Code":
1. ✅ Frontend console: `VNPay URL created with bankCode: VNPAYQR`
2. ✅ Network request: `{ amount: 100000, language: 'vn', returnUrl: '...', bankCode: 'VNPAYQR' }`
3. ✅ VNPay URL: `...&vnp_BankCode=VNPAYQR&...&vnp_SecureHash=...`
4. ✅ VNPay page: Hiển thị màn hình QR code trực tiếp (không phải trang chọn phương thức)

---

## ⚠️ LƯU Ý VỀ VNPAY SANDBOX

VNPay Sandbox có thể có giới hạn:
- Không hiển thị QR code trong test mode
- Chỉ hiển thị trang chọn phương thức
- Trong production sẽ hoạt động đầy đủ

Nếu sau khi kiểm tra tất cả các bước trên mà vẫn không hiển thị QR, có thể do giới hạn của VNPay Sandbox.

---

**Sau khi test, cho tôi biết kết quả từ console log và network request!** 🔍

