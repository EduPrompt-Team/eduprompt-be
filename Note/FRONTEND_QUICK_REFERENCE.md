# Frontend Quick Reference - API Changes

## 🚀 Endpoints Mới

### 1. Payment Methods (Public)
```
GET /api/payment-methods/public
→ Trả về danh sách payment methods active (không cần Admin)
```

### 2. Payment Status Check
```
GET /api/payments/check-package/{packageId}
→ Kiểm tra user đã thanh toán package chưa
→ Response: { packageId, isPaid, orderId?, paymentId?, ... }
```

### 3. Wishlist Check (StorageId)
```
GET /api/wishlists/check/{storageId}
→ Kiểm tra StorageTemplate có trong wishlist chưa
```

### 4. Wishlist Delete (StorageId)
```
DELETE /api/wishlists/by-storage/{storageId}
→ Xóa wishlist item theo StorageId
```

---

## 📝 Response Changes

### Order Response - Thêm PackageId
```json
{
  "orderId": 2,
  "packageId": 4,  // ✅ MỚI - Có thể null
  "status": "Completed",
  ...
}
```

### Wishlist Response - Thêm StorageTemplate Info
```json
{
  "wishlistId": 1,
  "storageId": 123,  // ✅ MỚI
  "templateName": "...",  // ✅ MỚI
  "grade": "12",  // ✅ MỚI
  "subject": "Toán",  // ✅ MỚI
  ...
}
```

### Review Response - Thêm PackageId
```json
{
  "reviewId": 1,
  "storageId": 123,
  "packageId": 456,  // ✅ MỚI - Optional
  "rating": 5,
  ...
}
```

---

## 🔄 Request Changes

### Wishlist Create
```json
// Trước: { packageId: 123 }
// Sau: { storageId: 123, packageId: 456? }  // storageId required
```

### Review Create
```json
// Trước: { storageId, rating, comment }
// Sau: { storageId, rating, comment, packageId? }  // packageId optional
// Comment limit: 1000 → 5000 characters
```

---

## ⚠️ Breaking Changes

1. **Wishlist API:**
   - `POST /api/wishlists` yêu cầu `storageId` (required)
   - Response format thay đổi (thêm StorageTemplate fields)

2. **Reviews API:**
   - Comment limit: 1000 → 5000 characters

---

## ✅ Non-Breaking Changes

1. Order response thêm `packageId` (có thể null)
2. Payment methods endpoint mới (không ảnh hưởng endpoint cũ)
3. Payment status check endpoint mới
4. Auto-create payment khi order completed

---

## 📋 Migration Checklist

- [ ] Update payment methods endpoint → `/api/payment-methods/public`
- [ ] Sử dụng `/api/payments/check-package/{packageId}` thay vì query nhiều endpoints
- [ ] Handle `packageId: null` trong order response
- [ ] Chuyển wishlist từ `packageId` sang `storageId`
- [ ] Update review comment limit: 1000 → 5000
- [ ] Test tất cả endpoints mới

---

**Xem chi tiết:** `FRONTEND_SYNC_CHANGES.md`

