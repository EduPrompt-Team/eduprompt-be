# 💳 TỔNG HỢP TẤT CẢ ENDPOINTS VÀ DTOs - PAYMENT SYSTEM

## 📋 Base URL
- **HTTP**: `http://localhost:5217`
- **HTTPS**: `https://localhost:7199`
- **Swagger**: `http://localhost:5217/swagger`

---

## 🔵 10. PAYMENT METHODS (Phương thức thanh toán)

**Route**: `/api/payment-methods`  
**Controller**: `PaymentMethodController`  
**Group**: `10. Payment Methods`

### Endpoints:

#### 1. GET /api/payment-methods
- **Mô tả**: Lấy tất cả payment methods (Admin only)
- **Auth**: `[Authorize(Policy = "AdminOnly")]`
- **Response**: `List<PaymentMethodDto>`
- **Response Code**: 200, 401, 403

#### 2. GET /api/payment-methods/user/{UserId}
- **Mô tả**: Lấy payment methods của user
- **Auth**: `[Authorize]`
- **Response**: `List<PaymentMethodDto>`
- **Response Code**: 200, 400, 401

#### 3. GET /api/payment-methods/{id}
- **Mô tả**: Lấy payment method theo ID
- **Auth**: `[Authorize]`
- **Response**: `PaymentMethodDto`
- **Response Code**: 200, 400, 401, 404

#### 4. POST /api/payment-methods
- **Mô tả**: Tạo payment method mới
- **Auth**: `[Authorize]`
- **Body**: `CreatePaymentMethodDto`
- **Response**: `PaymentMethodDto`
- **Response Code**: 201, 400, 401

#### 5. PUT /api/payment-methods/{id}
- **Mô tả**: Cập nhật payment method
- **Auth**: `[Authorize]`
- **Body**: `CreatePaymentMethodDto`
- **Response**: `PaymentMethodDto`
- **Response Code**: 200, 400, 401, 404

#### 6. DELETE /api/payment-methods/{id}
- **Mô tả**: Xóa payment method
- **Auth**: `[Authorize]`
- **Response**: 204 NoContent
- **Response Code**: 204, 400, 401, 404

#### 7. GET /api/payment-methods/user/{UserId}/default
- **Mô tả**: Lấy payment method mặc định của user
- **Auth**: `[Authorize]`
- **Response**: `PaymentMethodDto`
- **Response Code**: 200, 400, 401, 404

#### 8. POST /api/payment-methods/{id}/set-default?UserId={userId}
- **Mô tả**: Đặt payment method làm mặc định
- **Auth**: `[Authorize]`
- **Query**: `UserId` (int)
- **Response**: `{ message: "Payment method set as default successfully" }`
- **Response Code**: 200, 400, 401, 404

---

## 🟢 13. PAYMENTS (Giao dịch thanh toán)

**Route**: `/api/payments`  
**Controller**: `PaymentsController`  
**Group**: `13. Payments`

### Endpoints:

#### 1. GET /api/payments
- **Mô tả**: Lấy tất cả payments (Admin only)
- **Auth**: `[Authorize(Policy = "AdminOnly")]`
- **Response**: `List<PaymentServiceDto>`
- **Response Code**: 200, 401, 403

#### 2. GET /api/payments/{paymentId}
- **Mô tả**: Lấy payment theo ID
- **Auth**: `[Authorize]`
- **Response**: `PaymentServiceDto`
- **Response Code**: 200, 401, 404

#### 3. GET /api/payments/orders/{orderId}
- **Mô tả**: Lấy payments của order
- **Auth**: `[Authorize]`
- **Response**: `List<PaymentServiceDto>`
- **Response Code**: 200, 401

#### 4. POST /api/payments/orders/{orderId}/vnpay-url
- **Mô tả**: Tạo VNPay payment URL
- **Auth**: `[Authorize]`
- **Body**: `VnpayRequestServiceDto`
- **Response**: `{ url: string }`
- **Response Code**: 200, 400, 401
- **Note**: Tự động lấy userId từ token, IpAddr từ request

#### 5. GET /api/payments/vnpay-callback
- **Mô tả**: Callback từ VNPay sau khi thanh toán (Browser redirect)
- **Auth**: `[AllowAnonymous]`
- **Query**: `VnpayCallbackServiceDto` (từ VNPay)
- **Response**: `PaymentServiceDto`
- **Response Code**: 200

#### 6. POST /api/payments/vnpay-ipn
- **Mô tả**: IPN (Instant Payment Notification) từ VNPay (Server-to-Server)
- **Auth**: `[AllowAnonymous]`
- **Body**: `VnpayCallbackServiceDto` (FromForm)
- **Response**: `{ RspCode: "00", Message: "Confirm Success" }` hoặc `{ RspCode: "97", Message: "Invalid signature or data" }`
- **Response Code**: 200
- **Note**: VNPay gọi endpoint này để confirm payment

#### 7. POST /api/payments/querydr
- **Mô tả**: Query VNPay transaction (Admin only)
- **Auth**: `[Authorize(Policy = "AdminOnly")]`
- **Body**: `VnpayQueryRequestDto`
- **Response**: `object` (VNPay query response)
- **Response Code**: 200, 401, 403

#### 8. POST /api/payments/refund
- **Mô tả**: Hoàn tiền VNPay transaction (Admin only)
- **Auth**: `[Authorize(Policy = "AdminOnly")]`
- **Body**: `VnpayRefundRequestDto`
- **Response**: `object` (VNPay refund response)
- **Response Code**: 200, 401, 403

#### 9. POST /api/payments/orders/{orderId}/manual
- **Mô tả**: Tạo manual payment (COD hoặc offline) (Admin only)
- **Auth**: `[Authorize(Policy = "AdminOnly")]`
- **Body**: `PaymentCreateServiceDto`
- **Response**: `PaymentServiceDto`
- **Response Code**: 200, 400, 401, 403

#### 10. PATCH /api/payments/{paymentId}/status?status={status}
- **Mô tả**: Cập nhật status của payment (Admin only)
- **Auth**: `[Authorize(Policy = "AdminOnly")]`
- **Query**: `status` (string) - e.g., "Paid", "Pending", "Failed", "Refunded"
- **Response**: `PaymentServiceDto`
- **Response Code**: 200, 400, 401, 403

#### 11. POST /api/payments/wallets/{walletId}/topup
- **Mô tả**: Tạo VNPay payment URL cho wallet top-up (nạp tiền vào ví)
- **Auth**: `[Authorize]`
- **Body**: `WalletTopupRequestDto`
  ```json
  {
    "amount": 100000,        // Required, decimal > 0
    "bankCode": "NCB",        // Optional
    "language": "vn",         // Optional, default: "vn"
    "returnUrl": "..."       // Optional, override appsettings
  }
  ```
- **Response**: `{ url: string }` - VNPay payment URL
- **Response Code**: 200, 400, 401, 403
- **Note**: Tự động lấy userId từ token, IpAddr từ request. Sau khi thanh toán thành công, số tiền sẽ được nạp vào wallet.

#### 12. POST /api/payments/transactions/{transactionId}/vnpay-url
- **Mô tả**: Tạo VNPay payment URL cho transaction payment
- **Auth**: `[Authorize]`
- **Body**: `VnpayRequestServiceDto`
  ```json
  {
    "bankCode": "NCB",        // Optional
    "language": "vn",          // Optional, default: "vn"
    "returnUrl": "...",        // Optional
    "ipAddr": "..."            // Auto-filled from request if null
  }
  ```
- **Response**: `{ url: string }` - VNPay payment URL
- **Response Code**: 200, 400, 401, 403
- **Note**: Tự động lấy userId từ token, IpAddr từ request. Transaction phải thuộc về user hiện tại.

---

## 📦 DTOs

### PaymentMethodDto
```csharp
{
    "paymentMethodId": int,
    "methodName": string,
    "provider": string,
    "isActive": bool,
    "processingFee": decimal?
}
```

### CreatePaymentMethodDto
```csharp
{
    "methodName": string,        // Required, MaxLength(100)
    "provider": string,          // Required, MaxLength(50)
    "isActive": bool,            // Default: true
    "processingFee": decimal?     // Default: 0.00, Range(0, MaxValue)
}
```

### PaymentServiceDto
```csharp
{
    "paymentId": int,
    "orderId": int,
    "paymentMethod": string,
    "amount": decimal,
    "paymentDate": DateTime,
    "status": string,            // "Pending", "Paid", "Failed", "Refunded"
    "orderNumber": string?,
    "userName": string?,
    "userEmail": string?,
    "vnpayTransactionId": string?,
    "vnpayResponseCode": string?
}
```

### PaymentCreateServiceDto (Manual Payment)
```csharp
{
    "paymentMethod": string,     // e.g., "COD", "Bank Transfer"
    "amount": decimal,
    "provider": string           // Default: "VNPay"
}
```

### VnpayRequestServiceDto
```csharp
{
    "bankCode": string?,          // Optional, e.g., "NCB", "VIETCOMBANK"
    "language": string,           // Default: "vn"
    "returnUrl": string?,         // Optional, override appsettings
    "ipAddr": string?             // Auto-filled from request if null
}
```

### VnpayCallbackServiceDto
```csharp
{
    "vnp_TmnCode": string,
    "vnp_Amount": string,
    "vnp_BankCode": string,
    "vnp_BankTranNo": string,
    "vnp_CardType": string,
    "vnp_PayDate": string,
    "vnp_OrderInfo": string,
    "vnp_TransactionNo": string,
    "vnp_ResponseCode": string,      // "00" = Success
    "vnp_TransactionStatus": string,
    "vnp_TxnRef": string,
    "vnp_SecureHash": string
}
```

### VnpayQueryRequestDto
```csharp
{
    "txnRef": string,             // Required, vnp_TxnRef from payment
    "transactionDate": string,    // Required, format: yyyyMMddHHmmss (GMT+7)
    "orderInfo": string?,         // Optional
    "ipAddr": string?             // Auto-filled if null
}
```

### VnpayRefundRequestDto
```csharp
{
    "txnRef": string,             // Required
    "amount": string,             // Required, amount*100 (VNPay format)
    "transactionDate": string,    // Required, format: yyyyMMddHHmmss
    "createBy": string,            // Default: "system"
    "ipAddr": string?             // Auto-filled if null
}
```

### WalletTopupRequestDto
```csharp
{
    "amount": decimal,            // Required, > 0
    "bankCode": string?,          // Optional, e.g., "NCB", "VIETCOMBANK"
    "language": string?,          // Optional, default: "vn"
    "returnUrl": string?          // Optional, override appsettings ReturnUrl
}
```

---

## 💰 Payment Status Values

### Common Status:
- `Pending` - Chờ xử lý
- `Paid` - Đã thanh toán
- `Failed` - Thanh toán thất bại
- `Refunded` - Đã hoàn tiền
- `Cancelled` - Đã hủy

---

## 🔄 VNPay Payment Flow

### Flow 1: Order Payment

#### 1. User tạo order
```
POST /api/order/create-from-cart
```

#### 2. Tạo VNPay payment URL
```
POST /api/payments/orders/{orderId}/vnpay-url
Body: {
    "bankCode": "NCB",      // Optional
    "language": "vn"
}
Response: {
    "url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?..."
}
```

### Flow 2: Wallet Top-up

#### 1. User nạp tiền vào ví
```
POST /api/payments/wallets/{walletId}/topup
Body: {
    "amount": 100000,       // Required
    "bankCode": "NCB",      // Optional
    "language": "vn"        // Optional
}
Response: {
    "url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?..."
}
```

### Flow 3: Transaction Payment

#### 1. User thanh toán transaction
```
POST /api/payments/transactions/{transactionId}/vnpay-url
Body: {
    "bankCode": "NCB",      // Optional
    "language": "vn"        // Optional
}
Response: {
    "url": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?..."
}
```

#### 2. (Tất cả flows) User thanh toán trên VNPay
- Redirect user đến URL từ bước trước
- User thanh toán trên VNPay website
- VNPay redirect về `ReturnUrl` với query params

### Callback & IPN (Chung cho tất cả flows)

#### 3. VNPay Callback (Browser)
```
GET /api/payments/vnpay-callback
Query: vnp_TxnRef=xxx&vnp_ResponseCode=00&...
Response: PaymentServiceDto
```

#### 4. VNPay IPN (Server-to-Server)
```
POST /api/payments/vnpay-ipn
Body (FromForm): vnp_TxnRef=xxx&vnp_ResponseCode=00&...
Response: { RspCode: "00", Message: "Confirm Success" }
```

**Xử lý callback tự động:**
- **Wallet Top-up** (TxnRef bắt đầu với `WLT-`): Tự động nạp tiền vào wallet và tạo Transaction record với `TransactionType = "TopUp"`
- **Transaction Payment** (TxnRef bắt đầu với `TXN-`): Tạo Transaction record với `TransactionType = "ExternalPayment"`
- **Order Payment** (TxnRef bắt đầu với `ORD-`): Cập nhật Order status = "Paid" và tạo Transaction record

### Admin Functions

#### 5. Admin Query Transaction (Optional)
```
POST /api/payments/querydr
Body: {
    "txnRef": "xxx",
    "transactionDate": "20250117100000"
}
```

#### 6. Admin Refund (If needed)
```
POST /api/payments/refund
Body: {
    "txnRef": "xxx",
    "amount": "1000000",      // 10,000 VND * 100
    "transactionDate": "20250117100000"
}
```

---

## 🔗 Liên quan đến Payments

### Transactions (Giao dịch ví)
- Xem: `Note/ALL_API_ENDPOINTS.md` - Section 11. Transactions

### Orders (Đơn hàng)
- Xem: `Note/ALL_API_ENDPOINTS.md` - Section 14. Orders

### Wallet (Ví)
- Xem: `Note/ALL_API_ENDPOINTS.md` - Section 12. Wallet

---

## 📝 Ghi chú quan trọng

1. **Database Migration**: Cần chạy migration script `MIGRATE_Payment_OrderId_Nullable.sql` để cho phép `Payment.OrderId` nullable (hỗ trợ wallet top-up và transaction payment không có order).

2. **VNPay Configuration**: Cần cấu hình trong `appsettings.json`:
   - `VNPay:Url` - VNPay payment gateway URL
   - `VNPay:TmnCode` - Terminal code
   - `VNPay:HashSecret` - Hash secret key
   - `VNPay:ReturnUrl` - Callback URL

2. **Payment Status Flow**:
   ```
   Pending → Paid (via VNPay/Manual)
            ↓
         Failed
            ↓
         Refunded
   ```

3. **IPN vs Callback**:
   - **Callback**: User browser redirect (GET)
   - **IPN**: Server-to-server notification (POST FromForm)

4. **Amount Format**:
   - VNPay yêu cầu: `amount * 100` (VD: 10,000 VND = "1000000")
   - Trả về từ VNPay: chia cho 100

5. **Security**:
   - VNPay sử dụng SecureHash để verify request
   - IPN endpoint phải verify signature trước khi xử lý

---

**Updated**: 2025-01-17  
**Version**: 1.0

