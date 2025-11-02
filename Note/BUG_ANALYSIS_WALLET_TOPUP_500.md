# 🐛 BUG ANALYSIS: Wallet Top-up API Error 500

## 📋 PHÂN TÍCH NGUYÊN NHÂN

### ✅ **NGUYÊN NHÂN CHÍNH: Database Constraint Violation**

#### **Vấn đề 1: OrderId NOT NULL Constraint**
```sql
-- Database Schema hiện tại (từ DatabaseSchemaUpdater.cs)
OrderID INT NOT NULL,  -- ❌ NOT NULL

-- Foreign Key Constraint
FK_Payments_Orders FOREIGN KEY (OrderID) REFERENCES Orders(OrderID)
```

**Code đang cố insert:**
```csharp
OrderId = 0, // ❌ OrderId = 0 nhưng Orders table không có OrderId = 0
```

**Kết quả:**
- SQL Server từ chối insert vì `OrderId = 0` không tồn tại trong `Orders` table
- Foreign key constraint violation → `DbUpdateException` → **500 Internal Server Error**

---

### ✅ **Vấn đề 2: Entity Definition không nullable**

**Payment Entity:**
```csharp
public int OrderId { get; set; }  // ❌ Không nullable (int, không phải int?)
```

**Entity Framework mapping:**
- `int` → `NOT NULL` column trong database
- Không thể insert `NULL` hoặc giá trị không hợp lệ

---

### ✅ **Vấn đề 3: Migration chưa được chạy**

**Migration script đã có sẵn:** `Note/MIGRATE_Payment_OrderId_Nullable.sql`

**Nhưng có thể:**
- ❌ Chưa được chạy trên database
- ❌ Database vẫn giữ constraint cũ
- ❌ OrderId vẫn là `NOT NULL`

---

## 🔍 CÁC NGUYÊN NHÂN KHÁC (Ít khả năng hơn)

### 1. **VNPay Configuration Missing**
```csharp
var tmnCode = vnp["TmnCode"] ?? string.Empty;  // Empty string → có thể gây lỗi khi build URL
var hashSecret = vnp["HashSecret"] ?? string.Empty;  // Empty → signature sẽ sai
```

**Triệu chứng:**
- VNPay URL được tạo nhưng không hợp lệ
- Exception khi tạo signature
- Nhưng thường sẽ throw exception rõ ràng hơn

### 2. **Wallet Validation Exception**
```csharp
if (wallet == null) throw new InvalidOperationException("Wallet not found");
if (wallet.UserId != userId) throw new UnauthorizedAccessException("Wallet does not belong to user");
```

**Exception handler mapping:**
- `InvalidOperationException` → **400 Bad Request** (không phải 500)
- `UnauthorizedAccessException` → **401 Unauthorized** (không phải 500)

→ **Không phải nguyên nhân**

### 3. **Argument Exception**
```csharp
if (amount <= 0) throw new ArgumentException("Amount must be greater than 0", nameof(amount));
```

**Exception handler mapping:**
- `ArgumentException` → **400 Bad Request** (không phải 500)

→ **Không phải nguyên nhân**

---

## ✅ XÁC NHẬN NGUYÊN NHÂN

**Dựa trên error response:**
```json
{
  "statusCode": 500,
  "message": "An internal server error occurred. Please contact support.",
  "timestamp": "2025-11-02T14:59:42.5385158Z",
  "path": "/api/payments/wallets/1/topup"
}
```

**Exception handler chỉ trả về 500 khi:**
- Exception không match với bất kỳ case nào trong switch statement
- Có exception không được handle (unhandled exception)

**Các exception được handle:**
- `InvalidOperationException` → 400
- `ArgumentException` → 400
- `UnauthorizedAccessException` → 401
- `KeyNotFoundException` → 404
- `ForbiddenException` → 403

**→ Exception gây lỗi là `DbUpdateException` (không được handle) → 500**

---

## 🔧 GIẢI PHÁP

### **GIẢI PHÁP 1: Chạy Migration Script (ƯU TIÊN)**

```sql
-- Chạy file: Note/MIGRATE_Payment_OrderId_Nullable.sql
-- Hoặc chạy trực tiếp:
```

```powershell
# PowerShell
Invoke-Sqlcmd -ServerInstance "(local)" -Database "EdupromptV2" -Username "sa" -Password "123456" -InputFile "Note\MIGRATE_Payment_OrderId_Nullable.sql"
```

**Sau khi migration:**
- `OrderId` sẽ là `NULL` thay vì `0`
- Code cần sửa từ `OrderId = 0` → `OrderId = null` (hoặc không set, EF sẽ tự set NULL)

---

### **GIẢI PHÁP 2: Sửa Entity Definition**

**File:** `Eduprompt.Domain/Entities/Payment.cs`

```csharp
// TRƯỚC:
public int OrderId { get; set; }  // ❌

// SAU:
public int? OrderId { get; set; }  // ✅ Nullable
```

**Lưu ý:**
- Sau khi sửa, cần rebuild project
- Entity Framework sẽ nhận diện OrderId là nullable

---

### **GIẢI PHÁP 3: Sửa Code để set NULL thay vì 0**

**File:** `Eduprompt.BLL/Services/PaymentService.cs`

```csharp
// TRƯỚC:
var payment = new Payment
{
    OrderId = 0, // ❌
    // ...
};

// SAU:
var payment = new Payment
{
    OrderId = null, // ✅ Hoặc không set (EF sẽ tự set NULL)
    // ...
};
```

**Tại dòng 424 và 491 trong PaymentService.cs**

---

### **GIẢI PHÁP 4: Update DatabaseSchemaUpdater**

**File:** `Eduprompt.API/DependencyInjection/DatabaseSchemaUpdater.cs`

**Line 95:** Sửa từ `NOT NULL` → `NULL`:

```csharp
// TRƯỚC:
OrderID INT NOT NULL,  // ❌

// SAU:
OrderID INT NULL,  // ✅
```

**Và line 110:** Foreign key constraint đã đúng (nullable FK)

---

## 📝 CORRECTION VỀ EXPECTED BEHAVIOR

### **Bug Report nói sai:**

❌ **Sai:** "Tạo Transaction với TransactionType = 'TopUp' ngay khi tạo payment URL"

✅ **Đúng:** Transaction CHỈ được tạo khi VNPay callback thành công (`vnp_ResponseCode = "00"`)

**Flow đúng:**
1. ✅ Validate wallet exists và belongs to user
2. ✅ Validate amount > 0
3. ✅ Tạo Payment record với status = "Pending", OrderId = NULL
4. ✅ Generate VNPay payment URL
5. ✅ Return `{ url: string }`
6. **Sau callback thành công:**
   - Update Payment status = "Paid"
   - **TẠO Transaction** với TransactionType = "TopUp"
   - Nạp tiền vào wallet: `wallet.balance += amount`

---

## 🧪 TEST PLAN

### **Test 1: Verify Database Schema**

```sql
-- Kiểm tra OrderId có nullable không
SELECT 
    COLUMN_NAME, 
    IS_NULLABLE,
    DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Payments' 
  AND COLUMN_NAME = 'OrderID';

-- Expected: IS_NULLABLE = 'YES'
-- Nếu = 'NO' → Cần chạy migration
```

### **Test 2: Verify Foreign Key**

```sql
-- Kiểm tra FK constraint
SELECT 
    fk.name AS ForeignKey,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
    COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferencedColumn
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fc
    ON fk.object_id = fc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) = 'Payments'
  AND COL_NAME(fc.parent_column_id, fc.parent_column_id) = 'OrderID';

-- Expected: FK tồn tại và cho phép NULL
```

### **Test 3: Test Insert với NULL**

```sql
-- Test insert Payment với OrderId = NULL
INSERT INTO Payments (
    OrderID,  -- NULL
    UserID,
    Amount,
    PaymentMethod,
    Provider,
    Status,
    CreatedAt,
    TxnRef
)
VALUES (
    NULL,  -- ✅ Phải được phép
    1,
    100000,
    'Online',
    'VNPay',
    'Pending',
    GETUTCDATE(),
    'TEST-123'
);

-- Nếu thành công → Schema đã đúng
-- Nếu thất bại → Cần migration
```

### **Test 4: API Test sau khi fix**

```bash
# Test API
POST /api/payments/wallets/1/topup
Body: { "amount": 200000, "language": "vn" }

# Expected: 200 OK với VNPay URL
# Response: { "url": "https://sandbox.vnpayment.vn/..." }
```

---

## 🚨 URGENCY & PRIORITY

**Priority: CRITICAL** 🔴

**Impact:**
- ✅ Payment flow bị block hoàn toàn
- ✅ Users không thể nạp tiền vào ví
- ✅ Wallet top-up feature không hoạt động
- ✅ Revenue bị ảnh hưởng

**Estimated Fix Time:** 5-10 phút (chạy migration + rebuild)

---

## 📋 CHECKLIST ĐỂ FIX

- [ ] **1. Kiểm tra database schema hiện tại**
  ```sql
  SELECT IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS 
  WHERE TABLE_NAME = 'Payments' AND COLUMN_NAME = 'OrderID';
  ```

- [ ] **2. Chạy migration script**
  ```powershell
  Invoke-Sqlcmd -ServerInstance "(local)" -Database "EdupromptV2" -Username "sa" -Password "123456" -InputFile "Note\MIGRATE_Payment_OrderId_Nullable.sql"
  ```

- [ ] **3. Sửa Entity definition** (nếu cần)
  ```csharp
  public int? OrderId { get; set; }  // Nullable
  ```

- [ ] **4. Sửa PaymentService code**
  ```csharp
  OrderId = null,  // Thay vì OrderId = 0
  ```

- [ ] **5. Rebuild project**
  ```powershell
  dotnet build
  ```

- [ ] **6. Test API**
  ```bash
  POST /api/payments/wallets/1/topup
  ```

- [ ] **7. Verify database record**
  ```sql
  SELECT * FROM Payments WHERE TxnRef LIKE 'WLT-%' ORDER BY PaymentID DESC;
  -- Verify OrderID = NULL
  ```

---

## 📞 NEXT STEPS

1. **Backend Team:**
   - Chạy migration script ngay lập tức
   - Verify database schema sau migration
   - Test API endpoint
   - Deploy fix

2. **Frontend Team:**
   - Đợi backend fix
   - Retry API call sau khi backend deploy
   - Verify payment flow hoạt động

---

**Root Cause:** Database schema không cho phép `OrderId = NULL`, nhưng code đang cố insert với `OrderId = 0` (không tồn tại trong Orders table) → Foreign key constraint violation → 500 Internal Server Error.

**Fix:** Chạy migration để cho phép `OrderId` nullable, sau đó sửa code để set `OrderId = null` thay vì `0`.

---

**Updated:** 2025-01-17  
**Status:** 🔴 CRITICAL - Needs immediate fix

