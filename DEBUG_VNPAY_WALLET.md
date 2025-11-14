# 🔍 Debug VNPay Wallet Top-up Issue

## Vấn đề
Thanh toán VNPay thành công nhưng ví không cộng tiền.

## ✅ Đã sửa

1. **Backend Callback Endpoint** - Redirect về frontend sau khi xử lý
2. **Logging chi tiết** - Thêm logging để debug
3. **Error handling** - Xử lý lỗi tốt hơn

## 🔍 Cách Debug

### 1. Kiểm tra Backend Logs

Khi test thanh toán VNPay, xem logs trong terminal backend:

```
Created payment record - TxnRef: WLT-123-20251113120000, UserId: 1, Amount: 200000
VNPay Callback Received - TxnRef: WLT-123-20251113120000, ResponseCode: 00, Amount: 20000000
Found payment - PaymentId: 123, UserId: 1, Amount: 200000, TxnRef: WLT-123-20251113120000, Status: Pending
SUCCESS: Added 200000 to wallet for UserId: 1, TxnRef: WLT-123-20251113120000
VNPay Callback Processed - PaymentId: 123, Status: Paid
```

### 2. Các lỗi có thể gặp

#### ❌ ERROR: Payment not found for TxnRef
- **Nguyên nhân**: Payment record không được tạo khi tạo VNPay URL
- **Giải pháp**: Kiểm tra xem `CreateVnpayUrlForWalletTopupAsync` có tạo payment record không

#### ❌ ERROR: Payment.UserId is null
- **Nguyên nhân**: Payment record được tạo nhưng không có UserId
- **Giải pháp**: Kiểm tra code tạo payment record trong `CreateVnpayUrlForWalletTopupAsync`

#### ❌ ERROR: Failed to add funds to wallet
- **Nguyên nhân**: Wallet không tồn tại cho UserId này
- **Giải pháp**: Đảm bảo wallet đã được tạo trước khi nạp tiền

#### ❌ EXCEPTION: Error adding funds to wallet
- **Nguyên nhân**: Có lỗi khi cập nhật wallet (database error, etc.)
- **Giải pháp**: Kiểm tra database connection và wallet table

### 3. Kiểm tra Database

```sql
-- Kiểm tra payment record
SELECT * FROM Payments WHERE TxnRef LIKE 'WLT-%' ORDER BY CreatedAt DESC;

-- Kiểm tra wallet balance
SELECT * FROM Wallets WHERE UserId = [YOUR_USER_ID];

-- Kiểm tra transactions
SELECT * FROM Transactions WHERE TransactionType = 'TopUp' ORDER BY TransactionDate DESC;
```

### 4. Test Flow

1. **Tạo VNPay URL**:
   - Gọi API: `POST /api/payments/wallets/{walletId}/topup`
   - Kiểm tra log: `Created payment record - TxnRef: ...`
   - Lưu TxnRef để kiểm tra sau

2. **Thanh toán trên VNPay**:
   - Dùng account test của VNPay
   - Thanh toán thành công

3. **Callback từ VNPay**:
   - VNPay redirect về: `/api/payments/vnpay-callback`
   - Backend xử lý và redirect về frontend
   - Kiểm tra log: `VNPay Callback Received`, `SUCCESS: Added ... to wallet`

4. **Frontend nhận callback**:
   - Frontend nhận query params từ URL
   - Refresh wallet để lấy số dư mới
   - Redirect về `/wallet`

## 🛠️ Fix nếu vẫn lỗi

### Nếu Payment.UserId là null:
- Kiểm tra `CreateVnpayUrlForWalletTopupAsync` có set `UserId = userId` không
- Kiểm tra userId có được truyền đúng từ controller không

### Nếu Wallet không tồn tại:
- Đảm bảo wallet đã được tạo trước khi nạp tiền
- Hoặc tự động tạo wallet nếu chưa có (cần sửa code)

### Nếu AddFundsByUserIdAsync trả về false:
- Kiểm tra wallet có tồn tại không: `SELECT * FROM Wallets WHERE UserId = ?`
- Kiểm tra database connection

## 📝 Logs mẫu khi thành công

```
Created payment record - TxnRef: WLT-5-20251113134500, UserId: 1, Amount: 200000
VNPay Callback Received - TxnRef: WLT-5-20251113134500, ResponseCode: 00, Amount: 20000000
Found payment - PaymentId: 123, UserId: 1, Amount: 200000, TxnRef: WLT-5-20251113134500, Status: Pending
SUCCESS: Added 200000 to wallet for UserId: 1, TxnRef: WLT-5-20251113134500
VNPay Callback Processed - PaymentId: 123, Status: Paid
```

## 🚀 Test lại

1. Restart backend
2. Test thanh toán VNPay
3. Xem logs trong terminal backend
4. Kiểm tra wallet balance trong database hoặc frontend

