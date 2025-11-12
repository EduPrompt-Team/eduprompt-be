# Logic Consistency Review - 3 Flows

## Tổng quan
Review toàn bộ logic xử lý trong 3 flows chính để đảm bảo nhất quán, hợp lý và "ăn ý" với nhau.

---

## 1. CORE FLOW - Logic xử lý

### 1.1. Order Creation
**Service:** `OrderService.CreateOrderFromCartAsync()`
- **Order Status:** `"Pending"` ✅
- **Logic:** Order được tạo từ cart, chưa thanh toán → Status = "Pending"
- **Transaction:** Chưa tạo transaction (chờ thanh toán)

### 1.2. Wallet Payment (Thanh toán bằng ví)
**Service:** `OrderService.PayOrderWithWalletAsync()`
- **Order Status:** `"Pending"` → `"Paid"` ✅
- **Transaction Status:** `"Completed"` ✅
- **Transaction Type:** `"Payment"` ✅
- **Transaction WalletId:** Actual wallet ID ✅
- **Transaction OrderId:** Actual order ID ✅
- **Logic:** 
  1. Check order status = "Pending"
  2. Check wallet balance
  3. Deduct funds from wallet
  4. Update order status = "Paid"
  5. Create transaction với status = "Completed" (thanh toán thành công ngay)

### 1.3. VNPay Payment (Thanh toán qua VNPay)
**Service:** `PaymentService.CreateVnpayUrlForOrderAsync()` → `ProcessVnpayCallbackAsync()`
- **Payment Status:** `"Pending"` → `"Paid"` (sau callback thành công) ✅
- **Order Status:** `"Pending"` → `"Paid"` (sau callback thành công) ✅
- **Transaction Status:** `"Completed"` (sau callback thành công) ✅
- **Transaction Type:** `"ExternalPayment"` ✅
- **Logic:**
  1. Tạo Payment với status = "Pending"
  2. Redirect đến VNPay
  3. VNPay callback → Verify signature
  4. Nếu thành công:
     - Payment status = "Paid"
     - Order status = "Paid"
     - Tạo transaction với status = "Completed"

### 1.4. Wallet Top-up (Nạp tiền vào ví)

#### 4a. Test Mode (Thanh toán test)
**Frontend/Mobile:** `PaymentPrompt.tsx`
- **Transaction Status:** `"Completed"` ✅ (tạo ngay từ đầu)
- **Transaction Type:** `"TopUp"` ✅
- **Logic:** Test mode → thanh toán thành công ngay → status = "Completed"

#### 4b. VNPay Top-up
**Service:** `PaymentService.CreateVnpayUrlForWalletTopupAsync()` → `ProcessVnpayCallbackAsync()`
- **Payment Status:** `"Pending"` → `"Paid"` (sau callback) ✅
- **Transaction Status:** `"Completed"` (sau callback thành công) ✅
- **Transaction Type:** `"TopUp"` ✅
- **Logic:**
  1. Tạo Payment với status = "Pending"
  2. Redirect đến VNPay
  3. VNPay callback → Verify signature
  4. Nếu thành công:
     - Payment status = "Paid"
     - Add funds to wallet
     - Tạo transaction với status = "Completed"

---

## 2. SELL FLOW - Logic xử lý

### 2.1. Post Purchase (Mua template từ post)
**Service:** `PostService.PurchasePostAsync()`
- **Post Status:** `"Active"` → `"Sold"` ✅
- **Buyer Transaction Status:** `"Completed"` ✅
- **Seller Transaction Status:** `"Completed"` ✅
- **Buyer Transaction Type:** `"Payment"` ✅
- **Seller Transaction Type:** `"Deposit"` ✅
- **Transaction WalletId:** Actual wallet IDs ✅
- **Transaction PaymentMethodId:** Default (1) hoặc Wallet method ✅
- **Logic:**
  1. Lock post để tránh race condition
  2. Check buyer balance
  3. Deduct from buyer wallet
  4. Add to seller wallet
  5. Tạo 2 transactions:
     - Buyer: Type = "Payment", Status = "Completed"
     - Seller: Type = "Deposit", Status = "Completed"
  6. Create StorageTemplate for buyer
  7. Create PromptInstance for buyer
  8. Update post status = "Sold"

---

## 3. ADMIN FLOW - Logic xử lý

### 3.1. Template Architecture Purchase (Mua template từ architecture)
**Service:** `TemplateCommerceService.PurchaseTemplateAsync()`
- **Order Status:** `"Paid"` ✅ (thanh toán ngay bằng ví)
- **Buyer Transaction Status:** `"Completed"` ✅
- **Seller Transaction Status:** `"Completed"` ✅
- **Buyer Transaction Type:** `"Payment"` ✅ (nhất quán với PostService)
- **Seller Transaction Type:** `"Deposit"` ✅ (nhất quán với PostService)
- **Transaction WalletId:** Actual wallet IDs ✅
- **Transaction PaymentMethodId:** Wallet method hoặc default ✅
- **Transaction OrderId:** Buyer có OrderId, Seller không có ✅
- **Logic:**
  1. Get buyer và seller wallets
  2. Deduct from buyer, add to seller
  3. Create order với status = "Paid" (thanh toán ngay)
  4. Tạo 2 transactions:
     - Buyer: Type = "Payment", Status = "Completed", OrderId = order.OrderId
     - Seller: Type = "Deposit", Status = "Completed", OrderId = null
  5. Create StorageTemplate for buyer
  6. Create PromptInstance for buyer

---

## 4. TRANSACTION STATUS RULES - Tổng hợp

### 4.1. Status = "Pending"
- **Payment records:** Khi tạo VNPay URL, chờ callback
- **Order records:** Khi tạo order từ cart, chưa thanh toán
- **PromptInstance:** Khi tạo mới, chưa có output

### 4.2. Status = "Completed"
- **Transaction records:** 
  - ✅ Wallet payment (thanh toán bằng ví)
  - ✅ VNPay callback thành công (top-up hoặc order payment)
  - ✅ Test mode payment (top-up)
  - ✅ Post purchase (buyer và seller)
  - ✅ Template architecture purchase (buyer và seller)
- **PromptInstance:** Khi đã có output hoặc đã hoàn thành
- **AIHistory:** Khi đã tạo xong

### 4.3. Status = "Paid"
- **Order records:** Sau khi thanh toán thành công (wallet hoặc VNPay)
- **Payment records:** Sau khi VNPay callback thành công

### 4.4. Status = "Failed"
- **Payment records:** Khi VNPay callback thất bại hoặc signature không hợp lệ

### 4.5. Status = "Cancelled"
- **Order records:** Khi user hủy order

---

## 5. TRANSACTION TYPE RULES - Tổng hợp

### 5.1. "TopUp"
- **Usage:** Nạp tiền vào ví
- **Amount:** Positive (+)
- **WalletId:** Wallet của user nạp tiền
- **OrderId:** null (top-up không có order)

### 5.2. "Payment"
- **Usage:** Thanh toán (tiền đi ra)
- **Amount:** Positive (sẽ hiển thị với dấu -)
- **WalletId:** Wallet của người thanh toán
- **OrderId:** Có thể có (nếu thanh toán order) hoặc null (nếu mua template trực tiếp)

### 5.3. "Deposit"
- **Usage:** Nhận tiền (tiền vào)
- **Amount:** Positive (sẽ hiển thị với dấu +)
- **WalletId:** Wallet của người nhận tiền
- **OrderId:** null (seller không có order)

### 5.4. "ExternalPayment"
- **Usage:** Thanh toán qua VNPay (không dùng ví nội bộ)
- **Amount:** Positive
- **WalletId:** Wallet của user (để tracking)
- **OrderId:** Có thể có (nếu thanh toán order)

---

## 6. CONSISTENCY CHECKLIST

### ✅ Đã nhất quán:
1. **Transaction Status:**
   - Wallet payment → "Completed" ✅
   - VNPay callback thành công → "Completed" ✅
   - Test mode → "Completed" ✅
   - Post purchase → "Completed" ✅
   - Template architecture purchase → "Completed" ✅

2. **Transaction Type:**
   - Top-up → "TopUp" ✅
   - Payment → "Payment" ✅
   - Deposit → "Deposit" ✅
   - VNPay → "ExternalPayment" ✅

3. **Order Status:**
   - Create order → "Pending" ✅
   - Wallet payment → "Paid" ✅
   - VNPay payment → "Paid" ✅
   - Template purchase → "Paid" ✅

4. **Wallet Operations:**
   - Tất cả AddFunds/DeductFunds đều có transaction records ✅
   - Transaction có actual WalletId ✅
   - Transaction có actual PaymentMethodId ✅

5. **Time Zone:**
   - Backend: UTC ✅
   - Frontend/Mobile: Hiển thị với timeZone: 'Asia/Ho_Chi_Minh' ✅

---

## 7. VẤN ĐỀ ĐÃ SỬA

### 7.1. TemplateCommerceService
**Trước:**
- WalletId = 0, PaymentMethodId = 0, OrderId = 0 ❌
- TransactionType = "DEBIT"/"CREDIT" ❌
- Order status = "Completed" ❌

**Sau:**
- WalletId = Actual wallet IDs ✅
- PaymentMethodId = Wallet method hoặc default ✅
- OrderId = Actual order ID (buyer), null (seller) ✅
- TransactionType = "Payment"/"Deposit" ✅
- Order status = "Paid" ✅

### 7.2. Frontend/Mobile Test Mode
**Trước:**
- Transaction status = "Pending" → Update thành "Completed" ❌

**Sau:**
- Transaction status = "Completed" ngay từ đầu ✅

### 7.3. OrderService Wallet Payment
**Trước:**
- Không tạo transaction record ❌

**Sau:**
- Tạo transaction với status = "Completed" ✅

### 7.4. Time Zone Display
**Trước:**
- Hiển thị theo browser timezone (có thể sai) ❌

**Sau:**
- Hiển thị với timeZone: 'Asia/Ho_Chi_Minh' ✅

---

## 8. KẾT LUẬN

Tất cả logic trong 3 flows đã được review và sửa để đảm bảo:
- ✅ **Nhất quán:** Cùng một hành động → cùng status và type
- ✅ **Hợp lý:** Logic xử lý đúng với business flow
- ✅ **Ăn ý:** Các service hỗ trợ nhau, không conflict

**Tất cả transaction status và type đã được chuẩn hóa và nhất quán trong toàn bộ hệ thống.**

