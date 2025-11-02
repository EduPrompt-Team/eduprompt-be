# 💳 PROMPT CHO FRONTEND - PAYMENT SYSTEM

## 📋 TỔNG QUAN

Backend đã có **đầy đủ Payment System** với VNPay integration để xử lý:
1. **Order Payment** - Thanh toán đơn hàng (đã có từ trước)
2. **Wallet Top-up** - Nạp tiền vào ví (MỚI)
3. **Transaction Payment** - Thanh toán transaction (MỚI)
4. **Payment Methods Management** - Quản lý phương thức thanh toán
5. **Payment Query & Refund** - Admin functions

---

## 🆕 THAY ĐỔI MỚI

### Endpoints Mới Thêm:
1. `POST /api/payments/wallets/{walletId}/topup` - Nạp tiền vào ví qua VNPay
2. `POST /api/payments/transactions/{transactionId}/vnpay-url` - Thanh toán transaction qua VNPay

**Lợi ích:**
- ✅ Không cần tạo Order trước khi thanh toán
- ✅ Wallet top-up tự động nạp tiền sau khi payment thành công
- ✅ Hỗ trợ transaction payment trực tiếp

---

## 📚 TẤT CẢ ENDPOINTS PAYMENT

### 🔵 10. PAYMENT METHODS

**Base Route:** `/api/payment-methods`  
**Auth:** `[Authorize]` (trừ Admin endpoints)

#### 1. GET `/api/payment-methods`
- **Mô tả**: Lấy tất cả payment methods (Admin only)
- **Auth**: Admin only
- **Response**: `List<PaymentMethodDto>`

#### 2. GET `/api/payment-methods/user/{UserId}`
- **Mô tả**: Lấy payment methods của user
- **Response**: `List<PaymentMethodDto>`

#### 3. GET `/api/payment-methods/{id}`
- **Mô tả**: Lấy payment method theo ID
- **Response**: `PaymentMethodDto`

#### 4. POST `/api/payment-methods`
- **Mô tả**: Tạo payment method mới
- **Body**: `CreatePaymentMethodDto`
- **Response**: `PaymentMethodDto`

#### 5. PUT `/api/payment-methods/{id}`
- **Mô tả**: Cập nhật payment method
- **Body**: `CreatePaymentMethodDto`
- **Response**: `PaymentMethodDto`

#### 6. DELETE `/api/payment-methods/{id}`
- **Mô tả**: Xóa payment method
- **Response**: `204 NoContent`

#### 7. GET `/api/payment-methods/user/{UserId}/default`
- **Mô tả**: Lấy payment method mặc định của user
- **Response**: `PaymentMethodDto`

#### 8. POST `/api/payment-methods/{id}/set-default?UserId={userId}`
- **Mô tả**: Đặt payment method làm mặc định
- **Response**: `{ message: "..." }`

---

### 🟢 13. PAYMENTS

**Base Route:** `/api/payments`  
**Auth:** `[Authorize]` (trừ callback/IPN)

#### 1. GET `/api/payments`
- **Mô tả**: Lấy tất cả payments (Admin only)
- **Auth**: Admin only
- **Response**: `List<PaymentServiceDto>`

#### 2. GET `/api/payments/{paymentId}`
- **Mô tả**: Lấy payment theo ID
- **Response**: `PaymentServiceDto`

#### 3. GET `/api/payments/orders/{orderId}`
- **Mô tả**: Lấy payments của order
- **Response**: `List<PaymentServiceDto>`

#### 4. POST `/api/payments/orders/{orderId}/vnpay-url` ⭐
- **Mô tả**: Tạo VNPay payment URL cho order
- **Body**: `VnpayRequestServiceDto`
- **Response**: `{ url: string }`

#### 5. GET `/api/payments/vnpay-callback`
- **Mô tả**: Callback từ VNPay (Browser redirect)
- **Auth**: `[AllowAnonymous]`
- **Query**: VNPay callback params
- **Response**: `PaymentServiceDto`

#### 6. POST `/api/payments/vnpay-ipn`
- **Mô tả**: IPN từ VNPay (Server-to-Server)
- **Auth**: `[AllowAnonymous]`
- **Body**: VNPay callback (FromForm)
- **Response**: `{ RspCode: "00", Message: "..." }`

#### 7. POST `/api/payments/querydr` (Admin)
- **Mô tả**: Query VNPay transaction
- **Auth**: Admin only
- **Body**: `VnpayQueryRequestDto`

#### 8. POST `/api/payments/refund` (Admin)
- **Mô tả**: Hoàn tiền VNPay transaction
- **Auth**: Admin only
- **Body**: `VnpayRefundRequestDto`

#### 9. POST `/api/payments/orders/{orderId}/manual` (Admin)
- **Mô tả**: Tạo manual payment (COD/offline)
- **Auth**: Admin only
- **Body**: `PaymentCreateServiceDto`
- **Response**: `PaymentServiceDto`

#### 10. PATCH `/api/payments/{paymentId}/status?status={status}` (Admin)
- **Mô tả**: Cập nhật payment status
- **Auth**: Admin only
- **Query**: `status` (string)
- **Response**: `PaymentServiceDto`

#### 11. POST `/api/payments/wallets/{walletId}/topup` 🆕⭐
- **Mô tả**: Tạo VNPay URL để nạp tiền vào ví
- **Body**: `WalletTopupRequestDto`
  ```json
  {
    "amount": 100000,      // Required: > 0
    "bankCode": "NCB",     // Optional
    "language": "vn",      // Optional, default: "vn"
    "returnUrl": "..."     // Optional
  }
  ```
- **Response**: `{ url: string }`
- **Tự động**: Sau callback thành công → Nạp tiền vào wallet

#### 12. POST `/api/payments/transactions/{transactionId}/vnpay-url` 🆕⭐
- **Mô tả**: Tạo VNPay URL để thanh toán transaction
- **Body**: `VnpayRequestServiceDto`
- **Response**: `{ url: string }`

---

## 🔄 PAYMENT FLOWS

### Flow 1: Order Payment (Cũ)

```typescript
// 1. User tạo order
const order = await createOrder(cartItems);

// 2. Tạo VNPay URL
const response = await fetch(
  `${API_BASE}/api/payments/orders/${order.orderId}/vnpay-url`,
  {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      bankCode: 'NCB',  // Optional
      language: 'vn',
    }),
  }
);
const { url } = await response.json();

// 3. Redirect to VNPay
window.location.href = url;

// 4. Handle callback
// VNPay redirects to: /api/payments/vnpay-callback?vnp_TxnRef=ORD-1-...&vnp_ResponseCode=00&...
// → Backend tự động cập nhật Order status = "Paid"
```

### Flow 2: Wallet Top-up (MỚI) ⭐

```typescript
// 1. User nhập số tiền muốn nạp
const amount = 100000; // 100,000 VND
const walletId = userWallet.walletId;

// 2. Tạo VNPay URL
const response = await fetch(
  `${API_BASE}/api/payments/wallets/${walletId}/topup`,
  {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      amount,           // Required
      bankCode: 'NCB',  // Optional
      language: 'vn',   // Optional
      returnUrl: `${window.location.origin}/wallet/topup/callback`, // Optional
    }),
  }
);

if (!response.ok) {
  const error = await response.json();
  throw new Error(error.message || 'Failed to create payment URL');
}

const { url } = await response.json();

// 3. Redirect to VNPay
window.location.href = url;

// 4. Handle callback
// VNPay redirects to returnUrl hoặc default callback
// → Backend tự động:
//    - Cập nhật Payment status = "Paid"
//    - Nạp tiền vào wallet: wallet.balance += amount
//    - Tạo Transaction với TransactionType = "TopUp"

// 5. Refresh wallet balance
const updatedWallet = await fetchWallet(userId);
setWalletBalance(updatedWallet.balance);
```

### Flow 3: Transaction Payment (MỚI) ⭐

```typescript
// 1. User có transaction cần thanh toán
const transactionId = pendingTransaction.transactionId;

// 2. Tạo VNPay URL
const response = await fetch(
  `${API_BASE}/api/payments/transactions/${transactionId}/vnpay-url`,
  {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      bankCode: 'NCB',  // Optional
      language: 'vn',    // Optional
    }),
  }
);

const { url } = await response.json();

// 3. Redirect to VNPay
window.location.href = url;

// 4. Handle callback
// → Backend tự động tạo Transaction record
```

---

## 📦 DTOs & TYPES

### PaymentMethodDto
```typescript
interface PaymentMethodDto {
  paymentMethodId: number;
  methodName: string;
  provider: string;
  isActive: boolean;
  processingFee?: number;
}
```

### CreatePaymentMethodDto
```typescript
interface CreatePaymentMethodDto {
  methodName: string;      // Required, max 100 chars
  provider: string;        // Required, max 50 chars
  isActive: boolean;       // Default: true
  processingFee?: number;  // Default: 0, >= 0
}
```

### PaymentServiceDto
```typescript
interface PaymentServiceDto {
  paymentId: number;
  orderId: number;         // May be 0 for wallet top-up/transaction payment
  paymentMethod: string;
  amount: number;
  paymentDate: string;     // ISO DateTime
  status: string;           // "Pending" | "Paid" | "Failed" | "Refunded" | "Cancelled"
  orderNumber?: string;
  userName?: string;
  userEmail?: string;
  vnpayTransactionId?: string;
  vnpayResponseCode?: string;  // "00" = Success
}
```

### VnpayRequestServiceDto
```typescript
interface VnpayRequestServiceDto {
  bankCode?: string;       // Optional, e.g., "NCB", "VIETCOMBANK"
  language?: string;       // Optional, default: "vn"
  returnUrl?: string;     // Optional, override default
  ipAddr?: string;        // Auto-filled, don't send
}
```

### WalletTopupRequestDto 🆕
```typescript
interface WalletTopupRequestDto {
  amount: number;          // Required, > 0 (VND)
  bankCode?: string;      // Optional
  language?: string;       // Optional, default: "vn"
  returnUrl?: string;     // Optional
}
```

### VnpayCallbackServiceDto
```typescript
// From VNPay callback (query params or form data)
interface VnpayCallbackServiceDto {
  vnp_TmnCode: string;
  vnp_Amount: string;
  vnp_BankCode: string;
  vnp_BankTranNo: string;
  vnp_CardType: string;
  vnp_PayDate: string;
  vnp_OrderInfo: string;
  vnp_TransactionNo: string;
  vnp_ResponseCode: string;      // "00" = Success
  vnp_TransactionStatus: string;
  vnp_TxnRef: string;             // Format: "WLT-{id}-{timestamp}" | "TXN-{id}-{timestamp}" | "ORD-{id}-{timestamp}"
  vnp_SecureHash: string;
}
```

---

## 🎨 UI/UX RECOMMENDATIONS

### 1. Wallet Top-up Page

```tsx
// WalletTopupPage.tsx
const WalletTopupPage = () => {
  const [amount, setAmount] = useState<number>(100000);
  const [bankCode, setBankCode] = useState<string>('');
  const [loading, setLoading] = useState(false);
  const { walletId, userId } = useAuth();

  const handleTopup = async () => {
    if (amount < 10000) {
      alert('Số tiền tối thiểu là 10,000 VND');
      return;
    }

    setLoading(true);
    try {
      const response = await fetch(
        `${API_BASE}/api/payments/wallets/${walletId}/topup`,
        {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${getToken()}`,
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            amount,
            bankCode: bankCode || undefined,
            language: 'vn',
            returnUrl: `${window.location.origin}/wallet/topup/callback`,
          }),
        }
      );

      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Không thể tạo link thanh toán');
      }

      const { url } = await response.json();
      window.location.href = url; // Redirect to VNPay
    } catch (error) {
      console.error('Top-up error:', error);
      alert(error.message || 'Có lỗi xảy ra. Vui lòng thử lại.');
      setLoading(false);
    }
  };

  return (
    <div className="wallet-topup-page">
      <h2>Nạp tiền vào ví</h2>
      
      <div className="form-group">
        <label>Số tiền (VND)</label>
        <input
          type="number"
          min="10000"
          step="1000"
          value={amount}
          onChange={(e) => setAmount(Number(e.target.value))}
          placeholder="Nhập số tiền..."
        />
        <small>Tối thiểu: 10,000 VND</small>
      </div>

      <div className="form-group">
        <label>Ngân hàng (Tùy chọn)</label>
        <select
          value={bankCode}
          onChange={(e) => setBankCode(e.target.value)}
        >
          <option value="">Tất cả</option>
          <option value="NCB">NCB</option>
          <option value="VIETCOMBANK">Vietcombank</option>
          <option value="VIETINBANK">Vietinbank</option>
        </select>
      </div>

      <button
        onClick={handleTopup}
        disabled={loading || amount < 10000}
      >
        {loading ? 'Đang xử lý...' : 'Thanh toán qua VNPay'}
      </button>
    </div>
  );
};
```

### 2. Payment Callback Handler

```tsx
// PaymentCallbackPage.tsx
const PaymentCallbackPage = () => {
  const [result, setResult] = useState<{
    success: boolean;
    message: string;
    txnRef?: string;
  } | null>(null);
  const router = useRouter();

  useEffect(() => {
    const params = new URLSearchParams(window.location.search);
    const responseCode = params.get('vnp_ResponseCode');
    const txnRef = params.get('vnp_TxnRef');

    if (responseCode === '00') {
      // Payment successful
      let message = 'Thanh toán thành công!';
      
      if (txnRef?.startsWith('WLT-')) {
        message = 'Nạp tiền thành công! Số tiền đã được cộng vào ví.';
        // Refresh wallet after 2 seconds
        setTimeout(() => {
          router.push('/wallet');
        }, 2000);
      } else if (txnRef?.startsWith('TXN-')) {
        message = 'Thanh toán transaction thành công!';
        setTimeout(() => {
          router.push('/transactions');
        }, 2000);
      } else if (txnRef?.startsWith('ORD-')) {
        message = 'Thanh toán đơn hàng thành công!';
        setTimeout(() => {
          router.push('/orders');
        }, 2000);
      }

      setResult({ success: true, message, txnRef });
    } else {
      setResult({
        success: false,
        message: 'Thanh toán thất bại. Vui lòng thử lại.',
        txnRef,
      });
    }
  }, []);

  if (!result) {
    return <div>Đang xử lý...</div>;
  }

  return (
    <div className="payment-callback">
      {result.success ? (
        <>
          <div className="success-icon">✅</div>
          <h2>Thành công!</h2>
          <p>{result.message}</p>
          {result.txnRef && (
            <p className="txn-ref">Mã giao dịch: {result.txnRef}</p>
          )}
        </>
      ) : (
        <>
          <div className="error-icon">❌</div>
          <h2>Thất bại</h2>
          <p>{result.message}</p>
        </>
      )}
    </div>
  );
};
```

### 3. Transaction Payment Button

```tsx
// TransactionItem.tsx
const TransactionPaymentButton = ({ transaction }: { transaction: Transaction }) => {
  const [loading, setLoading] = useState(false);

  const handlePayment = async () => {
    if (transaction.status !== 'Pending') {
      alert('Transaction này không thể thanh toán');
      return;
    }

    setLoading(true);
    try {
      const response = await fetch(
        `${API_BASE}/api/payments/transactions/${transaction.transactionId}/vnpay-url`,
        {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${getToken()}`,
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            language: 'vn',
          }),
        }
      );

      if (!response.ok) {
        throw new Error('Không thể tạo link thanh toán');
      }

      const { url } = await response.json();
      window.location.href = url;
    } catch (error) {
      console.error('Payment error:', error);
      alert('Có lỗi xảy ra. Vui lòng thử lại.');
      setLoading(false);
    }
  };

  return (
    <button
      onClick={handlePayment}
      disabled={loading || transaction.status !== 'Pending'}
      className="pay-button"
    >
      {loading ? 'Đang xử lý...' : 'Thanh toán qua VNPay'}
    </button>
  );
};
```

---

## ⚠️ QUAN TRỌNG - CẦN LƯU Ý

### 1. **TxnRef Format - Phân biệt loại payment**
Backend sử dụng `vnp_TxnRef` để phân biệt:
- `WLT-{walletId}-{timestamp}` → Wallet Top-up
- `TXN-{transactionId}-{timestamp}` → Transaction Payment  
- `ORD-{orderId}-{timestamp}` → Order Payment

Frontend nên check prefix để:
- Hiển thị message phù hợp
- Redirect đến đúng trang sau callback
- Refresh đúng data (wallet/transaction/order)

### 2. **Wallet Top-up tự động nạp tiền**
- Sau callback thành công (`vnp_ResponseCode = "00"`), backend **TỰ ĐỘNG**:
  - Nạp tiền vào wallet: `wallet.balance += amount`
  - Tạo Transaction với `TransactionType = "TopUp"`
- Frontend chỉ cần refresh wallet balance để hiển thị số tiền mới
- **KHÔNG CẦN** gọi API nào khác để nạp tiền

### 3. **Callback URL Handling**
```typescript
// Nên set returnUrl cụ thể cho từng flow
const returnUrl = `${window.location.origin}/payment/callback?type=wallet-topup&walletId=${walletId}`;

// Hoặc dùng hash để identify
const returnUrl = `${window.location.origin}/payment/callback#wallet-topup`;
```

### 4. **Error Handling**
```typescript
try {
  const response = await fetch(...);
  
  if (!response.ok) {
    // Parse error message
    let errorMessage = 'Có lỗi xảy ra';
    try {
      const error = await response.json();
      errorMessage = error.message || errorMessage;
    } catch {
      // Fallback to status text
      errorMessage = response.statusText;
    }
    
    throw new Error(errorMessage);
  }
  
  const data = await response.json();
  return data;
} catch (error) {
  // Show user-friendly error
  showErrorToast(error.message);
  console.error('Payment error:', error);
}
```

### 5. **Payment Status Values**
```typescript
type PaymentStatus = 
  | 'Pending'    // Chờ xử lý
  | 'Paid'       // Đã thanh toán
  | 'Failed'     // Thất bại
  | 'Refunded'   // Đã hoàn tiền
  | 'Cancelled'; // Đã hủy
```

### 6. **Security Best Practices**
- ✅ Luôn gửi `Authorization: Bearer {token}` header
- ✅ Không lưu payment URL vào localStorage/cache
- ✅ Verify `vnp_ResponseCode` từ callback
- ✅ Handle network errors và timeouts
- ✅ Không expose sensitive payment data trong console

---

## 📊 SO SÁNH: CŨ vs MỚI

### OLD Flow (Order Payment):
```
Create Order → Get OrderId → POST /api/payments/orders/{orderId}/vnpay-url → Redirect
```

### NEW Flow (Wallet Top-up):
```
POST /api/payments/wallets/{walletId}/topup → Get URL → Redirect
```

**Lợi ích:**
- ✅ Đơn giản hơn (không cần tạo Order)
- ✅ Trực tiếp nạp vào wallet
- ✅ Phù hợp cho flow nạp tiền nhanh

---

## 🧪 TESTING CHECKLIST

### Wallet Top-up:
- [ ] Gọi API với amount > 0 → Success
- [ ] Gọi API với amount <= 0 → 400 Bad Request
- [ ] Gọi API với walletId không tồn tại → 404
- [ ] Gọi API với walletId của user khác → 403
- [ ] Thanh toán thành công → Wallet balance được cộng
- [ ] Thanh toán thất bại → Wallet balance không đổi

### Transaction Payment:
- [ ] Gọi API với transactionId hợp lệ → Success
- [ ] Gọi API với transactionId không tồn tại → 404
- [ ] Gọi API với transactionId của user khác → 403
- [ ] Thanh toán thành công → Transaction status updated

### Callback Handling:
- [ ] Test với `vnp_ResponseCode = "00"` → Success message
- [ ] Test với `vnp_ResponseCode != "00"` → Error message
- [ ] Test với TxnRef `WLT-*` → Wallet-related message
- [ ] Test với TxnRef `TXN-*` → Transaction-related message
- [ ] Test với TxnRef `ORD-*` → Order-related message

---

## 📞 SUPPORT & DOCUMENTATION

- **Swagger UI**: `http://localhost:5217/swagger` → Group "13. Payments"
- **Backend Docs**: `Note/TONG_HOP_PAYMENT_ENDPOINTS.md`
- **VNPay Integration**: `Note/VNPAY_SETUP_GUIDE.md`

---

## 🔄 MIGRATION CHECKLIST

Nếu bạn đang migrate từ code cũ:

1. **Update Payment Service**
   - Thêm functions cho wallet top-up và transaction payment
   - Remove dependency on Order creation for wallet top-up

2. **Update UI Components**
   - Thêm Wallet Top-up page/component
   - Update Transaction list để hiển thị payment button
   - Update Payment callback handler để support 3 loại payment

3. **Update State Management**
   - Wallet balance state sau top-up
   - Transaction status sau payment
   - Order status sau payment (existing)

4. **Update Routing**
   - `/wallet/topup` route
   - `/payment/callback` route với type parameter

5. **Test Thoroughly**
   - Test all payment flows
   - Test error cases
   - Test callback handling

---

**Updated**: 2025-01-17  
**Version**: 2.0 (Including new endpoints)

