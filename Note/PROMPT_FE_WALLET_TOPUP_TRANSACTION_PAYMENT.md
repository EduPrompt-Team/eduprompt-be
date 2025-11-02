# 💰 PROMPT CHO FRONTEND - WALLET TOP-UP & TRANSACTION PAYMENT

## 📋 TÓM TẮT THAY ĐỔI

Backend đã thêm **2 endpoints mới** để hỗ trợ thanh toán VNPay cho:
1. **Wallet Top-up** (Nạp tiền vào ví) - Không cần tạo Order
2. **Transaction Payment** (Thanh toán transaction) - Không cần tạo Order

Trước đây, để thanh toán qua VNPay, frontend phải:
- Tạo Order trước → Sau đó gọi `/api/payments/orders/{orderId}/vnpay-url`

Bây giờ, frontend có thể:
- Nạp tiền trực tiếp vào wallet mà không cần tạo Order
- Thanh toán transaction mà không cần tạo Order

---

## 🆕 ENDPOINTS MỚI

### 1. POST `/api/payments/wallets/{walletId}/topup`
**Tạo VNPay payment URL để nạp tiền vào ví**

#### Request:
```http
POST /api/payments/wallets/{walletId}/topup
Authorization: Bearer {token}
Content-Type: application/json
```

**Path Parameters:**
- `walletId` (int, required) - ID của wallet cần nạp tiền

**Request Body:**
```json
{
  "amount": 100000,        // Required: Số tiền nạp (VND), > 0
  "bankCode": "NCB",       // Optional: Mã ngân hàng (NCB, VIETCOMBANK, etc.)
  "language": "vn",        // Optional: Ngôn ngữ (vn/en), default: "vn"
  "returnUrl": "..."       // Optional: URL callback sau khi thanh toán (override default)
}
```

#### Response (200 OK):
```json
{
  "url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_Amount=10000000&vnp_TxnRef=WLT-1-20250117123456&..."
}
```

#### Response Errors:
- `400 Bad Request` - Amount <= 0, wallet không hợp lệ
- `401 Unauthorized` - Chưa đăng nhập
- `403 Forbidden` - Wallet không thuộc về user hiện tại
- `404 Not Found` - Wallet không tồn tại

---

### 2. POST `/api/payments/transactions/{transactionId}/vnpay-url`
**Tạo VNPay payment URL để thanh toán transaction**

#### Request:
```http
POST /api/payments/transactions/{transactionId}/vnpay-url
Authorization: Bearer {token}
Content-Type: application/json
```

**Path Parameters:**
- `transactionId` (int, required) - ID của transaction cần thanh toán

**Request Body:**
```json
{
  "bankCode": "NCB",       // Optional: Mã ngân hàng
  "language": "vn",        // Optional: Ngôn ngữ, default: "vn"
  "returnUrl": "...",      // Optional: URL callback
  "ipAddr": "..."          // Optional: Auto-filled từ request, không cần gửi
}
```

#### Response (200 OK):
```json
{
  "url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_Amount=5000000&vnp_TxnRef=TXN-123-20250117123456&..."
}
```

#### Response Errors:
- `400 Bad Request` - Transaction không hợp lệ
- `401 Unauthorized` - Chưa đăng nhập
- `403 Forbidden` - Transaction không thuộc về user hiện tại
- `404 Not Found` - Transaction không tồn tại

---

## 🔄 FLOW XỬ LÝ

### Flow 1: Wallet Top-up (Nạp tiền vào ví)

```
1. User nhập số tiền muốn nạp (ví dụ: 100,000 VND)
   ↓
2. Frontend gọi: POST /api/payments/wallets/{walletId}/topup
   Body: { "amount": 100000, "bankCode": "NCB" }
   ↓
3. Backend trả về VNPay payment URL
   ↓
4. Frontend redirect user đến VNPay URL
   window.location.href = response.url;
   ↓
5. User thanh toán trên VNPay website
   ↓
6. VNPay redirect về ReturnUrl (callback)
   GET /api/payments/vnpay-callback?vnp_TxnRef=WLT-1-20250117...&vnp_ResponseCode=00&...
   ↓
7. Backend tự động:
   - Cập nhật Payment status = "Paid"
   - Nạp tiền vào wallet (wallet.balance += amount)
   - Tạo Transaction record với TransactionType = "TopUp"
   ↓
8. Frontend hiển thị kết quả:
   - Nếu vnp_ResponseCode = "00" → Thành công
   - Nếu khác → Thất bại
   ↓
9. Frontend có thể refresh wallet balance:
   GET /api/wallet/user/{userId}
```

### Flow 2: Transaction Payment (Thanh toán transaction)

```
1. User có transaction cần thanh toán (ví dụ: transactionId = 123)
   ↓
2. Frontend gọi: POST /api/payments/transactions/123/vnpay-url
   Body: { "bankCode": "NCB" }
   ↓
3. Backend trả về VNPay payment URL
   ↓
4. Frontend redirect user đến VNPay URL
   ↓
5. User thanh toán trên VNPay
   ↓
6. VNPay redirect về callback
   ↓
7. Backend tự động:
   - Cập nhật Payment status = "Paid"
   - Tạo Transaction record với TransactionType = "ExternalPayment"
   ↓
8. Frontend hiển thị kết quả và refresh transaction list
```

---

## 📝 CODE EXAMPLE (JavaScript/TypeScript)

### Example 1: Wallet Top-up

```typescript
interface WalletTopupRequest {
  amount: number;
  bankCode?: string;
  language?: string;
  returnUrl?: string;
}

async function createWalletTopupPayment(
  walletId: number,
  amount: number,
  bankCode?: string
): Promise<string> {
  const response = await fetch(
    `${API_BASE_URL}/api/payments/wallets/${walletId}/topup`,
    {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${getAuthToken()}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        amount,
        bankCode: bankCode || undefined,
        language: 'vn',
      } as WalletTopupRequest),
    }
  );

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Failed to create payment URL');
  }

  const data = await response.json();
  return data.url; // VNPay payment URL
}

// Usage:
try {
  const paymentUrl = await createWalletTopupPayment(
    walletId,
    100000, // 100,000 VND
    'NCB' // Optional bank code
  );
  
  // Redirect to VNPay
  window.location.href = paymentUrl;
} catch (error) {
  console.error('Top-up failed:', error);
  alert('Không thể tạo link thanh toán. Vui lòng thử lại.');
}
```

### Example 2: Transaction Payment

```typescript
interface VnpayRequest {
  bankCode?: string;
  language?: string;
  returnUrl?: string;
}

async function createTransactionPaymentUrl(
  transactionId: number,
  bankCode?: string
): Promise<string> {
  const response = await fetch(
    `${API_BASE_URL}/api/payments/transactions/${transactionId}/vnpay-url`,
    {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${getAuthToken()}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        bankCode: bankCode || undefined,
        language: 'vn',
      } as VnpayRequest),
    }
  );

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message || 'Failed to create payment URL');
  }

  const data = await response.json();
  return data.url;
}

// Usage:
try {
  const paymentUrl = await createTransactionPaymentUrl(transactionId, 'NCB');
  window.location.href = paymentUrl;
} catch (error) {
  console.error('Payment failed:', error);
}
```

### Example 3: Handle VNPay Callback

```typescript
// Sau khi VNPay redirect về, parse query params
function handleVnpayCallback(): { success: boolean; message: string } {
  const params = new URLSearchParams(window.location.search);
  const responseCode = params.get('vnp_ResponseCode');
  const txnRef = params.get('vnp_TxnRef');

  if (responseCode === '00') {
    // Payment successful
    if (txnRef?.startsWith('WLT-')) {
      // Wallet top-up - Refresh wallet balance
      refreshWalletBalance();
      return {
        success: true,
        message: 'Nạp tiền thành công!',
      };
    } else if (txnRef?.startsWith('TXN-')) {
      // Transaction payment - Refresh transaction list
      refreshTransactions();
      return {
        success: true,
        message: 'Thanh toán thành công!',
      };
    } else if (txnRef?.startsWith('ORD-')) {
      // Order payment
      return {
        success: true,
        message: 'Thanh toán đơn hàng thành công!',
      };
    }
  }

  return {
    success: false,
    message: 'Thanh toán thất bại. Vui lòng thử lại.',
  };
}

// Call on callback page load
const result = handleVnpayCallback();
if (result.success) {
  showSuccessToast(result.message);
  // Navigate to wallet/transaction page after 2 seconds
  setTimeout(() => {
    router.push('/wallet');
  }, 2000);
} else {
  showErrorToast(result.message);
}
```

---

## 🎨 UI/UX RECOMMENDATIONS

### 1. Wallet Top-up Page
```
┌─────────────────────────────────┐
│  Nạp tiền vào ví                │
├─────────────────────────────────┤
│  Số tiền: [__________] VND      │
│  (Tối thiểu: 10,000 VND)       │
│                                 │
│  Ngân hàng: [Dropdown]          │
│  - NCB                          │
│  - VIETCOMBANK                  │
│  - Tất cả                       │
│                                 │
│  [  Thanh toán qua VNPay  ]     │
└─────────────────────────────────┘
```

**Validation:**
- Amount > 0 và >= 10,000 VND (hoặc minimum của bạn)
- Hiển thị loading khi đang tạo payment URL
- Disable button trong khi xử lý

### 2. Transaction Payment Button
```
┌─────────────────────────────────┐
│  Transaction #123               │
│  Số tiền: 50,000 VND           │
│  Trạng thái: Pending            │
│                                 │
│  [  Thanh toán qua VNPay  ]    │
└─────────────────────────────────┘
```

### 3. Payment Success Page
```
┌─────────────────────────────────┐
│  ✅ Thanh toán thành công!       │
│                                 │
│  Mã giao dịch: WLT-1-20250117...│
│  Số tiền: 100,000 VND           │
│  Thời gian: 17/01/2025 12:34:56 │
│                                 │
│  Số dư ví hiện tại: 500,000 VND │
│                                 │
│  [  Về trang ví  ]             │
└─────────────────────────────────┘
```

---

## ⚠️ LƯU Ý QUAN TRỌNG

### 1. **Wallet Top-up tự động nạp tiền**
- Sau khi VNPay callback thành công (`vnp_ResponseCode = "00"`), backend **tự động** nạp tiền vào wallet
- Frontend chỉ cần refresh wallet balance để hiển thị số tiền mới
- Không cần gọi API nào khác để nạp tiền

### 2. **TxnRef Format**
Backend sử dụng `TxnRef` để phân biệt loại payment:
- `WLT-{walletId}-{timestamp}` → Wallet Top-up
- `TXN-{transactionId}-{timestamp}` → Transaction Payment
- `ORD-{orderId}-{timestamp}` → Order Payment

Frontend có thể dùng để hiển thị message phù hợp.

### 3. **Callback URL**
- Nếu không gửi `returnUrl` trong request, backend sẽ dùng `VNPay:ReturnUrl` từ appsettings.json
- Khuyến nghị: Frontend gửi `returnUrl` cụ thể cho từng flow
  ```typescript
  const returnUrl = `${window.location.origin}/payment/callback?type=wallet-topup&walletId=${walletId}`;
  ```

### 4. **Error Handling**
- Luôn handle các HTTP errors (400, 401, 403, 404)
- Hiển thị message lỗi rõ ràng cho user
- Trong trường hợp timeout hoặc network error, có thể retry

### 5. **Security**
- Luôn gửi `Authorization: Bearer {token}` header
- Không lưu payment URL vào localStorage hoặc cache
- Verify `vnp_ResponseCode` và `vnp_SecureHash` từ callback (backend đã verify, nhưng frontend có thể double-check)

### 6. **Wallet Balance Update**
Sau khi top-up thành công, nên:
```typescript
// Option 1: Refresh wallet data
const wallet = await fetchWalletByUserId(userId);

// Option 2: Optimistic update + refresh
setWalletBalance(prev => prev + topupAmount);
const wallet = await fetchWalletByUserId(userId); // Confirm actual balance
```

---

## 📊 COMPARISON: OLD vs NEW

### OLD Flow (Order Payment):
```
Create Order → Get OrderId → POST /api/payments/orders/{orderId}/vnpay-url
```

### NEW Flow (Wallet Top-up):
```
POST /api/payments/wallets/{walletId}/topup → Get URL → Redirect
```

**Lợi ích:**
- ✅ Không cần tạo Order trước (giảm bước, đơn giản hóa)
- ✅ Trực tiếp nạp vào wallet
- ✅ Phù hợp cho flow nạp tiền đơn giản

---

## 🧪 TESTING

### Test Cases:

1. **Wallet Top-up thành công**
   - Gọi API với amount > 0
   - Redirect đến VNPay
   - Thanh toán thành công
   - Verify wallet balance được cộng đúng số tiền

2. **Wallet Top-up thất bại**
   - Gọi API với amount <= 0 → Expect 400
   - Gọi API với walletId không tồn tại → Expect 404
   - Gọi API với walletId của user khác → Expect 403

3. **Transaction Payment**
   - Gọi API với transactionId hợp lệ
   - Verify redirect và payment flow

---

## 📞 SUPPORT

Nếu có vấn đề hoặc câu hỏi:
1. Kiểm tra Swagger: `http://localhost:5217/swagger` → Group "13. Payments"
2. Xem file: `Note/TONG_HOP_PAYMENT_ENDPOINTS.md` để biết chi tiết đầy đủ
3. Liên hệ backend team nếu cần hỗ trợ

---

**Updated**: 2025-01-17  
**Version**: 1.0

