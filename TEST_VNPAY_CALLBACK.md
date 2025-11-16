# 🧪 Test VNPay Callback Flow

## Bước 1: Kiểm tra Backend đang chạy

Backend phải đang chạy và listen trên:
- `http://localhost:5217` (HTTP)
- `https://localhost:7199` (HTTPS)

## Bước 2: Test Flow

### 2.1. Tạo VNPay URL (Nạp tiền)

1. Mở frontend: `http://localhost:5173/wallet/topup`
2. Nhập số tiền (ví dụ: 200000)
3. Click "Thanh toán bằng VNPay"
4. **Xem logs backend** - Bạn sẽ thấy:
   ```
   Created payment record - TxnRef: WLT-5-20251113134500, UserId: 1, Amount: 200000
   ```

### 2.2. Thanh toán trên VNPay

1. Sau khi click, bạn sẽ được redirect đến VNPay sandbox
2. Dùng account test của VNPay để thanh toán
3. Thanh toán thành công

### 2.3. Callback từ VNPay

1. VNPay sẽ redirect về: `http://localhost:5217/api/payments/vnpay-callback?...`
2. **Xem logs backend** - Bạn sẽ thấy:
   ```
   VNPay Callback Received - TxnRef: WLT-5-20251113134500, ResponseCode: 00, Amount: 20000000
   Found payment - PaymentId: 123, UserId: 1, Amount: 200000, TxnRef: WLT-5-20251113134500, Status: Pending
   SUCCESS: Added 200000 to wallet for UserId: 1, TxnRef: WLT-5-20251113134500
   VNPay Callback Processed - PaymentId: 123, Status: Paid
   ```

3. Backend redirect về frontend: `http://localhost:5173/wallet/topup?vnp_ResponseCode=00&...`

### 2.4. Frontend xử lý

1. Frontend nhận callback params
2. Refresh wallet để lấy số dư mới
3. Hiển thị thông báo thành công
4. Redirect về `/wallet`

## 🔍 Kiểm tra nếu không thấy logs

### Nếu không thấy "Created payment record":
- Kiểm tra API call có thành công không
- Kiểm tra network tab trong browser
- Kiểm tra backend có nhận request không

### Nếu không thấy "VNPay Callback Received":
- **Vấn đề**: VNPay không redirect về backend
- **Nguyên nhân có thể**:
  1. ReturnUrl trong VNPay config không đúng
  2. VNPay sandbox không thể truy cập localhost
  3. Firewall/network blocking

### Nếu thấy "ERROR: Payment not found":
- Payment record không được tạo khi tạo VNPay URL
- Kiểm tra database: `SELECT * FROM Payments WHERE TxnRef LIKE 'WLT-%' ORDER BY CreatedAt DESC`

### Nếu thấy "ERROR: Failed to add funds to wallet":
- Wallet không tồn tại cho UserId này
- Kiểm tra database: `SELECT * FROM Wallets WHERE UserId = [YOUR_USER_ID]`

## 🛠️ Debug với ngrok (nếu VNPay không thể truy cập localhost)

Nếu VNPay sandbox không thể truy cập `localhost`, bạn cần dùng ngrok:

1. **Cài đặt ngrok**: https://ngrok.com/
2. **Chạy ngrok**:
   ```bash
   ngrok http 5217
   ```
3. **Lấy public URL** từ ngrok (ví dụ: `https://abc123.ngrok.io`)
4. **Cập nhật ReturnUrl** trong `appsettings.json`:
   ```json
   "ReturnUrl": "https://abc123.ngrok.io/api/payments/vnpay-callback"
   ```
5. **Restart backend**
6. **Test lại**

## 📝 Checklist

- [ ] Backend đang chạy
- [ ] Frontend đang chạy
- [ ] Đã test tạo VNPay URL
- [ ] Đã thấy log "Created payment record"
- [ ] Đã thanh toán trên VNPay
- [ ] Đã thấy log "VNPay Callback Received"
- [ ] Đã thấy log "SUCCESS: Added ... to wallet"
- [ ] Wallet balance được cập nhật
- [ ] Frontend hiển thị số dư mới

## 🚨 Nếu vẫn không hoạt động

Gửi cho tôi:
1. **Logs từ backend** khi test (từ lúc click "Thanh toán" đến khi redirect về)
2. **Network tab** từ browser (xem API calls)
3. **Database queries**:
   ```sql
   SELECT * FROM Payments WHERE TxnRef LIKE 'WLT-%' ORDER BY CreatedAt DESC LIMIT 5;
   SELECT * FROM Wallets WHERE UserId = [YOUR_USER_ID];
   SELECT * FROM Transactions WHERE TransactionType = 'TopUp' ORDER BY TransactionDate DESC LIMIT 5;
   ```

