# Fix Wishlist API 500 Error

## Vấn Đề

Frontend báo lỗi **500 Internal Server Error** khi gọi `POST /api/wishlists`.

## Nguyên Nhân Có Thể

1. **Database chưa có column `StorageID`** (khả năng cao nhất)
   - Code đã được update để dùng `StorageId`
   - Nhưng database chưa chạy migration script

2. **Foreign Key Constraint Violation**
   - `StorageId` không tồn tại trong bảng `StorageTemplates`
   - Hoặc foreign key chưa được tạo

3. **Null Reference Exception**
   - `IStorageTemplateRepository` không được inject đúng (đã kiểm tra, OK)

## Cách Fix

### Bước 1: Kiểm Tra Database Schema

Chạy script kiểm tra:
```sql
-- Mở file: Note/CHECK_WISHLIST_DATABASE.sql trong SSMS và chạy
```

**Kết quả mong đợi:**
- ✅ `StorageID column EXISTS`
- ✅ `Foreign key FK_Wishlists_StorageTemplates EXISTS`
- ✅ `Index IX_Wishlists_StorageID EXISTS`

### Bước 2: Chạy Migration Script (Nếu Thiếu)

Nếu thiếu column `StorageID`, chạy migration:

```sql
-- Mở file: Note/MIGRATE_Add_StorageId_To_Wishlists.sql trong SSMS
-- Nhấn F5 để execute
```

**Script sẽ:**
- Thêm column `StorageID` (nullable)
- Tạo foreign key `FK_Wishlists_StorageTemplates`
- Tạo index `IX_Wishlists_StorageID`
- Migrate dữ liệu cũ (nếu có)

### Bước 3: Kiểm Tra Logs Backend

Xem logs trong terminal/console nơi chạy `dotnet run` để biết lỗi cụ thể:

```bash
# Tìm dòng có "An unhandled exception occurred" hoặc stack trace
```

**Các lỗi thường gặp:**

1. **`Invalid column name 'StorageID'`**
   - → Chưa chạy migration script
   - → Fix: Chạy `MIGRATE_Add_StorageId_To_Wishlists.sql`

2. **`The INSERT statement conflicted with the FOREIGN KEY constraint`**
   - → `StorageId` không tồn tại trong `StorageTemplates`
   - → Fix: Kiểm tra `StorageId` trong request có đúng không

3. **`Cannot insert the value NULL into column 'StorageID'`**
   - → DbContext mapping chưa đúng
   - → Fix: Đã fix trong code, rebuild lại

### Bước 4: Test Lại

Sau khi chạy migration:

1. **Restart API:**
   ```bash
   # Dừng API (Ctrl+C)
   # Chạy lại
   dotnet run --project Eduprompt.API
   ```

2. **Test trên Swagger:**
   - `POST /api/wishlists`
   - Request body:
     ```json
     {
       "storageId": 10,  // Phải tồn tại trong StorageTemplates
       "packageId": null,  // Optional
       "notes": "test"
     }
     ```

3. **Kiểm tra Response:**
   - ✅ `201 Created` → Thành công
   - ❌ `400 Bad Request` → `StorageId` không tồn tại
   - ❌ `500 Internal Server Error` → Xem logs để biết lỗi cụ thể

## Debug Steps

### 1. Kiểm Tra StorageId Có Tồn Tại Không

```sql
-- Kiểm tra StorageTemplate có ID = 10 không
SELECT StorageID, UserID, PackageID, TemplateName, IsPublic
FROM StorageTemplates
WHERE StorageID = 10;
```

Nếu không có → Dùng `StorageId` khác hoặc tạo mới StorageTemplate.

### 2. Kiểm Tra UserId Từ Token

Token có `nameidentifier: "1"` → UserId = 1

Kiểm tra user có tồn tại:
```sql
SELECT UserId, Email, FullName, Status
FROM Users
WHERE UserId = 1;
```

### 3. Test Trực Tiếp Trên Database

```sql
-- Test insert thủ công
INSERT INTO Wishlists (UserId, PackageID, StorageID, AddedAt, Notes)
VALUES (1, NULL, 10, GETUTCDATE(), 'test');

-- Nếu thành công → Code có vấn đề
-- Nếu lỗi → Database có vấn đề (foreign key, constraint, etc.)
```

## Quick Fix Checklist

- [ ] Chạy `CHECK_WISHLIST_DATABASE.sql` để kiểm tra schema
- [ ] Chạy `MIGRATE_Add_StorageId_To_Wishlists.sql` nếu thiếu column
- [ ] Restart API sau khi chạy migration
- [ ] Kiểm tra `StorageId` trong request có tồn tại trong `StorageTemplates`
- [ ] Xem logs backend để biết lỗi cụ thể
- [ ] Test lại trên Swagger

## Expected Response (Success)

```json
{
  "wishlistId": 1,
  "userId": 1,
  "packageId": null,
  "storageId": 10,
  "addedAt": "2025-11-12T12:30:00Z",
  "notes": "test",
  "templateName": "...",
  "grade": "...",
  "subject": "...",
  ...
}
```

## Common Errors & Solutions

| Error | Cause | Solution |
|-------|-------|----------|
| `500 Internal Server Error` | Database chưa có `StorageID` | Chạy migration script |
| `400 Bad Request: StorageTemplate with ID X not found` | `StorageId` không tồn tại | Dùng `StorageId` hợp lệ |
| `400 Bad Request: StorageTemplate is already in your wishlist` | Đã có trong wishlist | OK, không cần fix |
| `401 Unauthorized` | Token không hợp lệ | Login lại để lấy token mới |

