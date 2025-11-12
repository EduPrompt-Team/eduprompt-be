# Tổng Hợp Thay Đổi Backend - Đồng Bộ Frontend

**Date:** 2025-01-17  
**Version:** Backend API Updates

---

## 📋 Tổng Quan Thay Đổi

### 1. ✅ Payment Methods API - Thêm Endpoint Public
### 2. ✅ Payment Status Check - Endpoint Mới
### 3. ✅ Order Response - Thêm PackageId
### 4. ✅ Auto-Create Payment - Khi Order Completed
### 5. ✅ Reviews/Feedback API - Fix Validation
### 6. ✅ Wishlist API - Link với StorageTemplates

---

## 1. Payment Methods API

### ✅ Thay Đổi

**Endpoint mới:**
```
GET /api/payment-methods/public
```

**Thay đổi:**
- Endpoint `GET /api/payment-methods` vẫn yêu cầu Admin role
- Endpoint mới `GET /api/payment-methods/public` cho phép authenticated users truy cập
- Trả về danh sách payment methods đang active

**Request:**
```http
GET /api/payment-methods/public
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "paymentMethodId": 1,
    "methodName": "Wallet",
    "provider": "Internal",
    "isActive": true,
    "processingFee": 0
  },
  {
    "paymentMethodId": 2,
    "methodName": "VNPay",
    "provider": "VNPay",
    "isActive": true,
    "processingFee": 0
  }
]
```

**Migration:**
- Frontend có thể thay đổi từ `GET /api/payment-methods` sang `GET /api/payment-methods/public`
- Hoặc giữ nguyên và handle 403 error (không khuyến nghị)

---

## 2. Payment Status Check API

### ✅ Endpoint Mới

**Endpoint:**
```
GET /api/payments/check-package/{packageId}
```

**Mục đích:**
- Kiểm tra user đã thanh toán package chưa
- Thay thế logic frontend phải gọi nhiều endpoints

**Request:**
```http
GET /api/payments/check-package/4
Authorization: Bearer {token}
```

**Response - Đã thanh toán:**
```json
{
  "packageId": 4,
  "isPaid": true,
  "orderId": 2,
  "paymentId": 1,
  "paidAt": "2025-01-15T10:30:00Z",
  "amount": 100000,
  "paymentMethod": "Wallet",
  "status": "Paid"
}
```

**Response - Chưa thanh toán:**
```json
{
  "packageId": 4,
  "isPaid": false
}
```

**Logic:**
- Kiểm tra tất cả orders `Completed/Paid` của user
- Tìm order có `PackageId = packageId`
- Kiểm tra payment status `Paid/Completed`
- Trả về `isPaid: true` nếu tìm thấy

**Migration:**
- Frontend có thể thay thế logic kiểm tra payment status bằng endpoint này
- Giảm số lượng API calls từ 2-3 xuống 1

---

## 3. Order Response - Thêm PackageId

### ✅ Thay Đổi

**Endpoints bị ảnh hưởng:**
- `GET /api/orders/my`
- `GET /api/orders/{orderId}`
- `GET /api/orders` (Admin)

**Response format mới:**
```json
{
  "orderId": 2,
  "userId": 1,
  "packageId": 4,  // ✅ MỚI - Có thể null nếu order từ cart có nhiều packages
  "orderNumber": "2",
  "totalAmount": 100000,
  "createdDate": "2025-01-15T10:30:00Z",
  "orderDate": "2025-01-15T10:30:00Z",
  "status": "Completed",
  "userName": "John Doe",
  "userEmail": "john@example.com",
  "items": [],
  "payments": [
    {
      "paymentId": 1,
      "orderId": 2,
      "paymentMethod": "Wallet",
      "amount": 100000,
      "paymentDate": "2025-01-15T10:30:00Z",
      "status": "Paid"
    }
  ]
}
```

**Lưu ý:**
- `packageId` có thể là `null` nếu:
  - Order từ cart có nhiều packages
  - Order cũ trong database không có PackageID
  - Order không phải là order mua package

**Migration:**
- Frontend có thể sử dụng `packageId` từ order response thay vì phải query riêng
- Cần handle `packageId: null` trong UI

---

## 4. Auto-Create Payment - Khi Order Completed

### ✅ Thay Đổi

**Logic mới:**
- Khi order status được update thành `"Completed"` hoặc `"Paid"`, backend tự động tạo payment record
- Payment record có:
  - `OrderId`: ID của order
  - `UserId`: ID của user
  - `Amount`: Tổng tiền của order
  - `PaymentMethod`: "Wallet" (mặc định)
  - `Provider`: "Internal"
  - `Status`: "Paid"

**Endpoint:**
```
PATCH /api/orders/{orderId}/status?status=Completed
```

**Kết quả:**
- Order status được update
- Payment record được tạo tự động (nếu chưa có)
- Frontend không cần tạo payment record thủ công

**Migration:**
- Frontend không cần gọi `POST /api/payments` sau khi order completed
- Payment record sẽ tự động xuất hiện trong order response

---

## 5. Reviews/Feedback API - Fix Validation

### ✅ Thay Đổi

**Endpoint:**
```
POST /api/reviews
POST /api/feedbacks
```

**Request format:**
```json
{
  "storageId": 123,
  "rating": 5,
  "comment": "Template này rất hay!",
  "packageId": 456  // ✅ MỚI - Optional
}
```

**Response format:**
```json
{
  "reviewId": 1,
  "storageId": 123,
  "userId": 10,
  "packageId": 456,  // ✅ MỚI - Có trong response
  "rating": 5,
  "comment": "Template này rất hay!",
  "createdAt": "2025-01-15T10:30:00Z",
  "user": {
    "userId": 10,
    "fullName": "John Doe",
    "email": "john@example.com"
  }
}
```

**Validation:**
- `storageId`: Required, phải tồn tại trong StorageTemplates
- `rating`: Required, 1-5
- `comment`: Optional, max 5000 characters (tăng từ 1000)
- `packageId`: Optional, nếu có phải tồn tại trong Packages

**Error responses:**
- `404`: `"StorageTemplate with ID {id} not found"`
- `404`: `"Package with ID {id} not found"`
- `401`: `"User not found"`
- `400`: `"You have already reviewed this template"`

**Migration:**
- Frontend có thể gửi `packageId` trong request (optional)
- Frontend nhận `packageId` trong response
- Comment length limit tăng lên 5000 characters

---

## 6. Wishlist API - Link với StorageTemplates

### ✅ Thay Đổi Lớn

**Endpoint:**
```
POST /api/wishlists
GET /api/wishlists/my-wishlist
GET /api/wishlists/check/{storageId}
DELETE /api/wishlists/by-storage/{storageId}
```

**Request format mới:**
```json
{
  "storageId": 123,  // ✅ Required - ID của StorageTemplate
  "packageId": 456,  // Optional - Cho backward compatibility
  "notes": "Template này hay"
}
```

**Response format mới:**
```json
{
  "wishlistId": 1,
  "userId": 10,
  "packageId": null,  // Có thể null
  "storageId": 123,   // ✅ MỚI
  "addedAt": "2025-01-15T10:30:00Z",
  "notes": null,
  "templateName": "Toán Học Lớp 12 - Chương 1",  // ✅ MỚI
  "templateContent": "...",  // ✅ MỚI
  "grade": "12",  // ✅ MỚI
  "subject": "Toán",  // ✅ MỚI
  "chapter": "Chương 1",  // ✅ MỚI
  "isPublic": true,  // ✅ MỚI
  "templateCreatedAt": "2025-01-10T08:00:00Z"  // ✅ MỚI
}
```

**Endpoints mới:**
- `GET /api/wishlists/check/{storageId}` - Kiểm tra StorageTemplate có trong wishlist chưa
- `DELETE /api/wishlists/by-storage/{storageId}` - Xóa theo StorageId

**Endpoints legacy (backward compatibility):**
- `GET /api/wishlists/check/package/{packageId}` - Vẫn hoạt động

**Migration:**
- Frontend cần chuyển từ `packageId` sang `storageId` khi thêm vào wishlist
- Frontend nhận thông tin đầy đủ của StorageTemplate trong response
- Frontend có thể sử dụng endpoint mới `check/{storageId}` thay vì `check/package/{packageId}`

---

## 7. Order Query Parameters - Filtering

### ✅ Thay Đổi

**Endpoint:**
```
GET /api/orders/my?status=Completed&paid=true
```

**Query parameters mới:**
- `status`: Filter theo order status (e.g., "Completed", "Paid", "Pending")
- `paid`: Filter theo payment status
  - `paid=true`: Chỉ orders đã thanh toán
  - `paid=false`: Chỉ orders chưa thanh toán
  - Không có: Tất cả orders

**Ví dụ:**
```http
GET /api/orders/my?status=Completed&paid=true
```

**Response:**
```json
[
  {
    "orderId": 2,
    "packageId": 4,
    "status": "Completed",
    "payments": [
      {
        "status": "Paid",
        ...
      }
    ]
  }
]
```

**Migration:**
- Frontend có thể sử dụng query parameters để filter orders
- Giảm logic filtering ở frontend

---

## 📊 Tóm Tắt Breaking Changes

### ⚠️ Breaking Changes

1. **Wishlist API:**
   - `POST /api/wishlists` bây giờ yêu cầu `storageId` (required)
   - `packageId` là optional (backward compatibility)
   - Response format thay đổi (thêm nhiều fields từ StorageTemplate)

2. **Reviews API:**
   - Comment length limit tăng từ 1000 lên 5000 characters
   - Error messages thay đổi (rõ ràng hơn)

### ✅ Non-Breaking Changes

1. **Payment Methods:**
   - Endpoint mới `GET /api/payment-methods/public` (không ảnh hưởng endpoint cũ)

2. **Payment Status Check:**
   - Endpoint mới `GET /api/payments/check-package/{packageId}` (không ảnh hưởng endpoints cũ)

3. **Order Response:**
   - Thêm field `packageId` (có thể null, không breaking)

4. **Auto-Create Payment:**
   - Logic tự động, không ảnh hưởng API contract

---

## 🔄 Migration Guide cho Frontend

### 1. Payment Methods

**Trước:**
```javascript
// Có thể bị 403
const response = await fetch('/api/payment-methods', {
  headers: { 'Authorization': `Bearer ${token}` }
});
```

**Sau:**
```javascript
// Sử dụng endpoint public
const response = await fetch('/api/payment-methods/public', {
  headers: { 'Authorization': `Bearer ${token}` }
});
```

### 2. Payment Status Check

**Trước:**
```javascript
// Phải gọi nhiều endpoints
const orders = await fetch('/api/orders/my');
const order = orders.find(o => o.packageId === packageId);
const payments = await fetch(`/api/payments/orders/${order.orderId}`);
const isPaid = payments.some(p => p.status === 'Paid');
```

**Sau:**
```javascript
// Chỉ cần 1 endpoint
const response = await fetch(`/api/payments/check-package/${packageId}`, {
  headers: { 'Authorization': `Bearer ${token}` }
});
const { isPaid } = await response.json();
```

### 3. Order Response

**Trước:**
```javascript
// packageId có thể không có trong response
const order = await fetch('/api/orders/2');
// Phải query riêng để lấy packageId
```

**Sau:**
```javascript
// packageId có trong response (có thể null)
const order = await fetch('/api/orders/2');
const { packageId } = order; // Có thể null
```

### 4. Wishlist API

**Trước:**
```javascript
// Sử dụng packageId
await fetch('/api/wishlists', {
  method: 'POST',
  body: JSON.stringify({ packageId: 123 })
});
```

**Sau:**
```javascript
// Sử dụng storageId (required)
await fetch('/api/wishlists', {
  method: 'POST',
  body: JSON.stringify({ 
    storageId: 123,  // Required
    packageId: 456   // Optional
  })
});

// Check wishlist
const response = await fetch(`/api/wishlists/check/${storageId}`);
const { isInWishlist } = await response.json();
```

### 5. Reviews API

**Trước:**
```javascript
// Comment limit 1000
await fetch('/api/reviews', {
  method: 'POST',
  body: JSON.stringify({
    storageId: 123,
    rating: 5,
    comment: "..." // Max 1000 chars
  })
});
```

**Sau:**
```javascript
// Comment limit 5000, có thể gửi packageId
await fetch('/api/reviews', {
  method: 'POST',
  body: JSON.stringify({
    storageId: 123,
    rating: 5,
    comment: "...", // Max 5000 chars
    packageId: 456  // Optional
  })
});
```

---

## 📝 Test Cases cho Frontend

### Test Case 1: Payment Methods
```javascript
// Test endpoint public
const response = await fetch('/api/payment-methods/public', {
  headers: { 'Authorization': `Bearer ${token}` }
});
// Expected: 200 OK với danh sách payment methods
```

### Test Case 2: Payment Status Check
```javascript
// Test check package payment
const response = await fetch('/api/payments/check-package/4', {
  headers: { 'Authorization': `Bearer ${token}` }
});
const data = await response.json();
// Expected: { packageId: 4, isPaid: true/false, ... }
```

### Test Case 3: Order Response
```javascript
// Test order response có packageId
const response = await fetch('/api/orders/my', {
  headers: { 'Authorization': `Bearer ${token}` }
});
const orders = await response.json();
// Expected: orders[0].packageId có giá trị hoặc null
```

### Test Case 4: Wishlist với StorageId
```javascript
// Test add to wishlist với storageId
const response = await fetch('/api/wishlists', {
  method: 'POST',
  headers: { 
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({ storageId: 123 })
});
// Expected: 201 Created với wishlist item có templateName, etc.
```

### Test Case 5: Reviews với PackageId
```javascript
// Test create review với packageId
const response = await fetch('/api/reviews', {
  method: 'POST',
  headers: { 
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    storageId: 123,
    rating: 5,
    comment: "Great template!",
    packageId: 456
  })
});
// Expected: 201 Created với review có packageId trong response
```

---

## 🚨 Lưu Ý Quan Trọng

### 1. PackageId trong Order Response

- `packageId` có thể là `null` nếu:
  - Order từ cart có nhiều packages
  - Order cũ trong database không có PackageID
  - Order không phải là order mua package

**Frontend cần handle:**
```javascript
if (order.packageId) {
  // Order có packageId
} else {
  // Order không có packageId (có thể từ cart hoặc order cũ)
}
```

### 2. Wishlist API Migration

- Frontend cần chuyển từ `packageId` sang `storageId`
- Endpoint `check/package/{packageId}` vẫn hoạt động (backward compatibility)
- Nhưng khuyến nghị sử dụng `check/{storageId}` mới

### 3. Error Handling

- Error messages đã được cải thiện, rõ ràng hơn
- Status codes đã được sửa đúng (404, 401, 400)
- Frontend có thể hiển thị error messages trực tiếp cho user

---

## 📞 Liên Hệ

Nếu có câu hỏi hoặc cần hỗ trợ, vui lòng liên hệ backend team.

---

**Last Updated:** 2025-01-17

