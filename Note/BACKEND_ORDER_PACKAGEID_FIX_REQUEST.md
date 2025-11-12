# Yêu Cầu Backend: Fix Order PackageId và Payment Check

## Vấn Đề Hiện Tại

### 1. Endpoint `/api/payments/check-package/{packageId}` không trả về đúng `isPaid`

**Vấn đề:**
- Endpoint hiện tại chỉ kiểm tra orders có `PackageId` trực tiếp
- Không kiểm tra orders từ cart (có thể có nhiều packages trong một order)
- Logic kiểm tra payment status có thể không chính xác

**Yêu cầu:**
- Kiểm tra tất cả orders `Completed/Paid` của user
- Tìm order có chứa `packageId` này (trong `PackageId` hoặc trong order items)
- Trả về `isPaid: true` nếu có payment `Paid/Completed`

### 2. Order Response thiếu `packageId`

**Vấn đề:**
- `GET /api/orders/my` không trả về `packageId` trong response
- `GET /api/orders/{orderId}` không trả về `packageId` trong response
- Frontend cần `packageId` để hiển thị thông tin package và kiểm tra payment status

**Yêu cầu:**
- Thêm `packageId` vào order response trong `GET /api/orders/my`
- Thêm `packageId` vào order detail trong `GET /api/orders/{orderId}`
- Hoặc đảm bảo `packageId` có trong `items` array (nếu có order items)

## Yêu Cầu Backend

### 1. Sửa Endpoint `/api/payments/check-package/{packageId}`

**Logic hiện tại (cần sửa):**
```csharp
// Hiện tại chỉ kiểm tra orders có PackageId trực tiếp
var orders = await _orderRepository.GetByUserIdAndPackageIdAsync(userId, packageId);
```

**Logic mới (yêu cầu):**
```csharp
// 1. Lấy tất cả orders Completed/Paid của user
var allOrders = await _orderRepository.GetByUserIdAsync(userId);
var completedOrders = allOrders.Where(o => 
    o.Status == "Completed" || o.Status == "Paid"
).ToList();

// 2. Tìm order có chứa packageId này
//    - Kiểm tra PackageId trực tiếp
//    - Hoặc kiểm tra trong OrderItems (nếu có)
var orderWithPackage = completedOrders.FirstOrDefault(o => 
    o.PackageId == packageId || 
    (o.OrderItems != null && o.OrderItems.Any(item => item.PackageId == packageId))
);

// 3. Kiểm tra payment status
if (orderWithPackage != null)
{
    var payments = await _paymentRepository.GetByOrderIdAsync(orderWithPackage.OrderId);
    var paidPayment = payments.FirstOrDefault(p => 
        p.Status == "Paid" || p.Status == "Completed"
    );
    
    if (paidPayment != null)
    {
        return new PackagePaymentStatusDto
        {
            PackageId = packageId,
            IsPaid = true,
            OrderId = orderWithPackage.OrderId,
            PaymentId = paidPayment.PaymentId,
            PaidAt = paidPayment.CreatedAt,
            Amount = paidPayment.Amount,
            PaymentMethod = paidPayment.PaymentMethod,
            Status = paidPayment.Status
        };
    }
}

// 4. Nếu không tìm thấy, trả về isPaid = false
return new PackagePaymentStatusDto
{
    PackageId = packageId,
    IsPaid = false
};
```

**Lưu ý:**
- Nếu order có status `Completed` hoặc `Paid` nhưng chưa có payment record, vẫn có thể coi là đã thanh toán (tùy business logic)
- Hoặc chỉ trả về `isPaid: true` khi có payment record với status `Paid/Completed`

### 2. Thêm `packageId` vào Order Response

**File cần sửa:**
- `Eduprompt.Domain/Interface/Service/IOrderService.cs` - `OrderServiceDto`
- `Eduprompt.BLL/Services/OrderService.cs` - `MapToServiceDto()`

**Thay đổi:**

#### 2.1. Cập nhật `OrderServiceDto`

```csharp
public class OrderServiceDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime OrderDate { get; set; }
    public string? Status { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public List<OrderItemServiceDto>? Items { get; set; }
    public List<PaymentServiceDto>? Payments { get; set; }
    
    // ✅ Thêm PackageId
    public int? PackageId { get; set; }
}
```

#### 2.2. Cập nhật `MapToServiceDto()` trong `OrderService`

```csharp
private static OrderServiceDto MapToServiceDto(Order order)
{
    return new OrderServiceDto
    {
        OrderId = order.OrderId,
        UserId = order.UserId,
        OrderNumber = order.OrderId.ToString(),
        TotalAmount = order.TotalAmount,
        CreatedDate = order.OrderDate,
        OrderDate = order.OrderDate,
        Status = order.Status,
        UserName = order.User?.FullName,
        UserEmail = order.User?.Email,
        Items = new List<OrderItemServiceDto>(),
        Payments = order.Payments?.Select(p => new PaymentServiceDto
        {
            PaymentId = p.PaymentId,
            OrderId = p.OrderId ?? 0,
            PaymentMethod = p.PaymentMethod,
            Amount = p.Amount,
            PaymentDate = p.CreatedAt,
            Status = p.Status,
            VnpayTransactionId = p.TransactionNo,
            VnpayResponseCode = p.ResponseCode
        }).ToList() ?? new List<PaymentServiceDto>(),
        
        // ✅ Thêm PackageId
        PackageId = order.PackageId
    };
}
```

**Lưu ý:**
- `Order` entity đã có `PackageId` (nullable `int?`)
- Chỉ cần map từ entity sang DTO
- Nếu order có nhiều packages (từ cart), có thể cần thêm logic để lấy `packageId` từ `OrderItems`

### 3. Kiểm Tra Order Items (Nếu Cần)

**Nếu orders có thể chứa nhiều packages (từ cart):**

Cần kiểm tra xem có `OrderItems` table không. Nếu có:
- Thêm `PackageId` vào `OrderItemServiceDto`
- Map `PackageId` từ `OrderItems` trong `MapToServiceDto()`

**Ví dụ:**
```csharp
Items = order.OrderItems?.Select(item => new OrderItemServiceDto
{
    OrderDetailId = item.OrderDetailId,
    OrderId = item.OrderId,
    PackageId = item.PackageId,  // ✅ Thêm PackageId
    Quantity = item.Quantity,
    Price = item.Price,
    SubTotal = item.SubTotal
}).ToList() ?? new List<OrderItemServiceDto>()
```

## Database Schema Reference

### Orders Table
```sql
CREATE TABLE [dbo].[Orders] (
    [OrderId] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [PackageID] INT NULL,  -- ✅ Đã có PackageID
    [TotalAmount] DECIMAL(18,2) NOT NULL,
    [OrderDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Notes] NVARCHAR(MAX) NULL,
    [Status] NVARCHAR(50) NULL DEFAULT 'Pending',
    ...
);
```

### Payments Table
```sql
CREATE TABLE [dbo].[Payments] (
    [PaymentID] INT IDENTITY(1,1) PRIMARY KEY,
    [OrderID] INT NULL,
    [UserID] INT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [Status] NVARCHAR(50) NULL,
    [PaymentMethod] NVARCHAR(50) NULL,
    ...
);
```

## Test Cases

### Test Case 1: Check Package Payment - Order có PackageId trực tiếp

```
GET /api/payments/check-package/123
User: userId = 10

Database:
- Order: OrderId=1, UserId=10, PackageID=123, Status="Completed"
- Payment: PaymentID=1, OrderID=1, Status="Paid"

Expected Response:
{
  "packageId": 123,
  "isPaid": true,
  "orderId": 1,
  "paymentId": 1,
  "paidAt": "2025-01-15T10:30:00Z",
  "amount": 100000,
  "paymentMethod": "Wallet",
  "status": "Paid"
}
```

### Test Case 2: Check Package Payment - Order từ Cart (nhiều packages)

```
GET /api/payments/check-package/123
User: userId = 10

Database:
- Order: OrderId=1, UserId=10, PackageID=NULL, Status="Completed"
- OrderItem: OrderDetailId=1, OrderId=1, PackageId=123
- Payment: PaymentID=1, OrderID=1, Status="Paid"

Expected Response:
{
  "packageId": 123,
  "isPaid": true,
  "orderId": 1,
  "paymentId": 1,
  ...
}
```

### Test Case 3: Check Package Payment - Chưa thanh toán

```
GET /api/payments/check-package/123
User: userId = 10

Database:
- Order: OrderId=1, UserId=10, PackageID=123, Status="Pending"
- Payment: Không có

Expected Response:
{
  "packageId": 123,
  "isPaid": false
}
```

### Test Case 4: Get Orders - Có PackageId trong response

```
GET /api/orders/my
User: userId = 10

Expected Response:
[
  {
    "orderId": 1,
    "userId": 10,
    "packageId": 123,  // ✅ Có PackageId
    "totalAmount": 100000,
    "status": "Completed",
    "orderDate": "2025-01-15T10:30:00Z",
    "payments": [...]
  }
]
```

### Test Case 5: Get Order Detail - Có PackageId trong response

```
GET /api/orders/1
User: userId = 10

Expected Response:
{
  "orderId": 1,
  "userId": 10,
  "packageId": 123,  // ✅ Có PackageId
  "totalAmount": 100000,
  "status": "Completed",
  "orderDate": "2025-01-15T10:30:00Z",
  "items": [...],
  "payments": [...]
}
```

## Files Cần Sửa

### 1. Payment Service
- `Eduprompt.BLL/Services/PaymentService.cs`
  - Method: `CheckPackagePaymentAsync()`
  - Logic: Kiểm tra tất cả orders Completed/Paid, tìm order có packageId

### 2. Order Service
- `Eduprompt.Domain/Interface/Service/IOrderService.cs`
  - Class: `OrderServiceDto`
  - Thêm: `public int? PackageId { get; set; }`

- `Eduprompt.BLL/Services/OrderService.cs`
  - Method: `MapToServiceDto()`
  - Thêm: `PackageId = order.PackageId`

### 3. Order Repository (Nếu Cần)
- `Eduprompt.Domain/Interface/Repository/IOrderRepository.cs`
  - Có thể cần method mới để lấy orders với OrderItems

- `Eduprompt.DAL/Repositories/OrderRepository.cs`
  - Có thể cần include OrderItems trong queries

## Ưu Tiên

1. **Cao**: Sửa endpoint `/api/payments/check-package/{packageId}` để trả về đúng `isPaid`
2. **Cao**: Thêm `packageId` vào order response
3. **Trung bình**: Kiểm tra OrderItems nếu orders có thể chứa nhiều packages

## Lợi Ích

- Frontend có thể kiểm tra payment status chính xác cho từng package
- Frontend có thể hiển thị thông tin package trong order list
- Logic payment check nhất quán và đáng tin cậy
- Trải nghiệm người dùng tốt hơn

## Notes

- Nếu orders có thể chứa nhiều packages (từ cart), cần kiểm tra cả `OrderItems` table
- Nếu không có `OrderItems` table, chỉ cần kiểm tra `PackageId` trực tiếp trong `Orders` table
- Đảm bảo `PackageId` là nullable trong response (có thể là `null` nếu order từ cart)

## Liên Hệ

Nếu có câu hỏi hoặc cần làm rõ thêm, vui lòng liên hệ frontend team.

