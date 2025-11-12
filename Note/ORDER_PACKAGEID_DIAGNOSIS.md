# Chẩn Đoán và Sửa Lỗi Order PackageId

## Vấn Đề

Frontend báo:
- Order có `status: "Completed"` nhưng `packageId: null`
- Order có `items: []` (rỗng)
- Order có `payments: []` (rỗng)
- Template đang tìm `packageId: 4` nhưng không tìm thấy trong bất kỳ order nào

## Nguyên Nhân Có Thể

### 1. Orders được tạo từ Cart (không có PackageId trực tiếp)

**Logic hiện tại:**
```csharp
// OrderService.CreateOrderFromCartAsync()
var order = new Order
{
    UserId = UserId,
    PackageId = null,  // ⚠️ NULL - vì cart có thể có nhiều packages
    TotalAmount = totalAmount,
    OrderDate = DateTime.UtcNow,
    Notes = notes,
    Status = "Pending"
};
```

**Vấn đề:**
- Orders từ cart không có `PackageId` trực tiếp
- `PackageId` có thể nằm trong `OrderItems` (nếu có table này)
- Hoặc cần logic khác để xác định package từ cart items

### 2. Orders cũ trong Database thiếu PackageId

**Có thể:**
- Orders được tạo trước khi có logic set PackageId
- Orders từ cart không được set PackageId
- Migration data không đầy đủ

### 3. API không trả về PackageId (ĐÃ SỬA)

**Đã sửa:**
- ✅ `OrderServiceDto` đã có `PackageId` property
- ✅ `MapToServiceDto()` đã map `PackageId = order.PackageId`

**Nhưng:**
- Nếu `order.PackageId` là `null` trong database → API sẽ trả về `null`

## Giải Pháp

### Bước 1: Kiểm Tra Database

**Chạy script:** `Note/CHECK_AND_FIX_ORDER_PACKAGEID.sql`

Script này sẽ:
1. Kiểm tra Order 2 có PackageID không
2. Kiểm tra tất cả orders của User 1
3. Kiểm tra orders có PackageID = 4
4. Kiểm tra payments của Order 2
5. Kiểm tra Package 4 có tồn tại không

### Bước 2: Xác Định Nguồn Gốc Order

**Câu hỏi:**
1. Order 2 được tạo từ đâu?
   - Từ cart? → Cần kiểm tra Cart/CartDetails
   - Từ direct package purchase? → Cần có PackageId
   - Từ admin? → Có thể thiếu PackageId

2. Order 2 có liên quan đến Package 4 không?
   - Kiểm tra lịch sử giao dịch
   - Kiểm tra StorageTemplates của user
   - Kiểm tra context khác

### Bước 3: Sửa Dữ Liệu (Nếu Cần)

**Option 1: Update Order với PackageID**

```sql
-- Nếu bạn biết chắc order này là mua package 4
UPDATE Orders
SET PackageID = 4
WHERE OrderId = 2 
  AND PackageID IS NULL
  AND Status IN ('Completed', 'Paid');
```

**Option 2: Tạo Order Mới**

Nếu order 2 không phải là order mua package, tạo order mới:

```sql
-- Tạo order mới với PackageID = 4
INSERT INTO Orders (UserId, PackageID, TotalAmount, Status, OrderDate)
SELECT 
    1,  -- UserId
    4,  -- PackageID
    Price,  -- TotalAmount từ Packages
    'Completed',  -- Status
    GETUTCDATE()  -- OrderDate
FROM Packages
WHERE PackageId = 4;
```

**Option 3: Tạo Payment Record**

Nếu order thiếu payment record:

```sql
INSERT INTO Payments (OrderID, UserID, Amount, Status, PaymentMethod, Provider, CreatedAt)
SELECT 
    OrderId,
    UserId,
    TotalAmount,
    'Paid',
    'Wallet',
    'Internal',
    OrderDate
FROM Orders
WHERE OrderId = 2
  AND NOT EXISTS (
      SELECT 1 
      FROM Payments p 
      WHERE p.OrderID = 2
  );
```

### Bước 4: Sửa Logic Tạo Order (Nếu Cần)

**Nếu orders từ cart cần có PackageId:**

Cần xác định:
- Cart có 1 package → Set PackageId
- Cart có nhiều packages → Set PackageId = null hoặc package đầu tiên

**Code đề xuất:**

```csharp
public async Task<OrderServiceDto> CreateOrderFromCartAsync(int UserId, string? notes)
{
    var cart = await _cartRepository.GetByUserIdAsync(UserId);
    var totalAmount = cart?.CartDetails?.Sum(cd => cd.Quantity * cd.UnitPrice) ?? 0m;

    // Xác định PackageId từ cart
    int? packageId = null;
    if (cart?.CartDetails != null && cart.CartDetails.Any())
    {
        // Lấy PackageId từ cart detail đầu tiên (hoặc logic khác)
        var firstCartDetail = cart.CartDetails.FirstOrDefault();
        if (firstCartDetail?.PackageId != null)
        {
            packageId = firstCartDetail.PackageId;
        }
    }

    var order = new Order
    {
        UserId = UserId,
        PackageId = packageId,  // ✅ Set PackageId nếu có
        TotalAmount = totalAmount,
        OrderDate = DateTime.UtcNow,
        Notes = notes,
        Status = "Pending"
    };

    var created = await _orderRepository.CreateAsync(order);
    await _cartRepository.ClearCartAsync(UserId);
    return MapToServiceDto(created);
}
```

## Kiểm Tra API

### Test 1: GET /api/orders/my

**Expected:**
```json
[
  {
    "orderId": 2,
    "userId": 1,
    "packageId": 4,  // ✅ Có PackageId
    "status": "Completed",
    ...
  }
]
```

### Test 2: GET /api/orders/2

**Expected:**
```json
{
  "orderId": 2,
  "userId": 1,
  "packageId": 4,  // ✅ Có PackageId
  "status": "Completed",
  ...
}
```

### Test 3: GET /api/payments/check-package/4

**Expected:**
```json
{
  "packageId": 4,
  "isPaid": true,
  "orderId": 2,
  "paymentId": 1,
  ...
}
```

## Checklist

- [ ] Chạy script `CHECK_AND_FIX_ORDER_PACKAGEID.sql` để kiểm tra database
- [ ] Xác định nguồn gốc Order 2 (từ cart hay direct purchase)
- [ ] Kiểm tra Order 2 có liên quan đến Package 4 không
- [ ] Sửa dữ liệu nếu cần (update PackageID hoặc tạo order mới)
- [ ] Test API endpoints để đảm bảo trả về PackageId
- [ ] Test endpoint `/api/payments/check-package/4` để đảm bảo hoạt động đúng

## Files Đã Sửa

1. ✅ `Eduprompt.Domain/Interface/Service/IOrderService.cs` - Thêm `PackageId` vào `OrderServiceDto`
2. ✅ `Eduprompt.BLL/Services/OrderService.cs` - Map `PackageId` trong `MapToServiceDto()`
3. ✅ `Eduprompt.BLL/Services/PaymentService.cs` - Cải thiện logic `CheckPackagePaymentAsync()`

## Next Steps

1. **Chạy SQL script** để kiểm tra database
2. **Xác định** nguồn gốc order và packageId cần thiết
3. **Sửa dữ liệu** nếu cần (update hoặc tạo mới)
4. **Test API** để đảm bảo hoạt động đúng

