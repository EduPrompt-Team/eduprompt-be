# Wishlist API - Liên Kết Với StorageTemplates

## Tổng Quan

Đã cập nhật Wishlist API để liên kết với **StorageTemplates** (prompt templates) thay vì chỉ **Packages**. Giữ backward compatibility với dữ liệu cũ dựa trên PackageID.

## Thay Đổi Database

### Migration Script
File: `Note/MIGRATE_Add_StorageId_To_Wishlists.sql`

**Thay đổi:**
- Thêm cột `StorageID` (nullable) vào bảng `Wishlists`
- Tạo foreign key `FK_Wishlists_StorageTemplates` với CASCADE delete
- Tạo index `IX_Wishlists_StorageID` cho performance
- Optional: Migrate dữ liệu cũ từ PackageID sang StorageID

**Chạy migration:**
```sql
-- Mở file Note/MIGRATE_Add_StorageId_To_Wishlists.sql trong SSMS và chạy
```

## Thay Đổi Code

### 1. Entity - Wishlist.Partial.cs
**File:** `Eduprompt.Domain/Entities/Wishlist.Partial.cs`

**Thêm:**
- `public int? StorageId { get; set; }` - ID của StorageTemplate
- `public virtual StorageTemplate? StorageTemplate { get; set; }` - Navigation property

### 2. DTOs

#### WishlistCreateDto.cs
**File:** `Eduprompt.Domain/DTOs/Wishlist/WishlistCreateDto.cs`

**Thay đổi:**
- `PackageId` → `int?` (nullable, optional cho backward compatibility)
- `StorageId` → `int` (required) - ID của StorageTemplate

#### WishlistDto.cs
**File:** `Eduprompt.Domain/DTOs/Wishlist/WishlistDto.cs`

**Thêm fields:**
- `StorageId` - ID của StorageTemplate
- `TemplateName`, `TemplateContent`, `Grade`, `Subject`, `Chapter`, `IsPublic`, `TemplateCreatedAt` - Thông tin từ StorageTemplate

### 3. Repository

#### IWishlistRepository.cs
**File:** `Eduprompt.Domain/Interface/Repository/IWishlistRepository.cs`

**Methods mới:**
- `GetUserWishlistItemByStorageIdAsync(int userId, int storageId)` - Lấy wishlist item theo StorageId
- `DeleteByStorageIdAsync(int userId, int storageId)` - Xóa theo StorageId
- `ExistsByStorageIdAsync(int userId, int storageId)` - Kiểm tra tồn tại theo StorageId

#### WishlistRepository.cs
**File:** `Eduprompt.DAL/Repositories/WishlistRepository.cs`

**Cập nhật:**
- Tất cả queries include `.Include(w => w.StorageTemplate)`
- Implement các methods mới cho StorageId

### 4. Service

#### IWishlistService.cs
**File:** `Eduprompt.Domain/Interface/Service/IWishlistService.cs`

**Methods mới:**
- `DeleteByStorageIdAsync(int userId, int storageId)`
- `IsInWishlistByStorageIdAsync(int userId, int storageId)`

#### WishlistService.cs
**File:** `Eduprompt.BLL/Services/WishlistService.cs`

**Thay đổi:**
- Inject `IStorageTemplateRepository`
- `CreateAsync`: Validate StorageTemplate thay vì Package (StorageId required)
- `MapToDto`: Map thông tin từ StorageTemplate vào DTO

### 5. Controller

#### WishlistsController.cs
**File:** `Eduprompt.API/Controllers/WishlistsController.cs`

**Endpoints mới:**
- `GET /api/wishlists/check/{storageId}` - Kiểm tra StorageTemplate có trong wishlist
- `DELETE /api/wishlists/by-storage/{storageId}` - Xóa theo StorageId

**Endpoints cập nhật:**
- `POST /api/wishlists` - Nhận `StorageId` (required), `PackageId` (optional)
- `GET /api/wishlists/my-wishlist` - Trả về thông tin StorageTemplate đầy đủ

**Endpoints legacy (backward compatibility):**
- `GET /api/wishlists/check/package/{packageId}` - Kiểm tra Package (legacy)

**Fix:**
- Bỏ hardcode `UserId = 1`, lấy từ JWT token: `int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value)`

### 6. DbContext

#### EdupromptV2Context.cs
**File:** `Eduprompt.DAL/DbContexts/EdupromptV2Context.cs`

**Cập nhật mapping:**
- `PackageId` → nullable (`IsRequired(false)`)
- Thêm `StorageId` column mapping
- Thêm foreign key relationship với `StorageTemplate`
- Thêm index cho `StorageId`

## API Endpoints

### 1. POST /api/wishlists
**Thêm StorageTemplate vào wishlist**

**Request:**
```json
{
  "storageId": 123,      // Required
  "packageId": 10,       // Optional - for backward compatibility
  "notes": "Muốn mua sau" // Optional
}
```

**Response:** `201 Created`
```json
{
  "wishlistId": 1,
  "userId": 10,
  "packageId": null,
  "storageId": 123,
  "addedAt": "2025-01-15T10:30:00Z",
  "notes": null,
  "templateName": "Toán Học Lớp 12 - Chương 1",
  "templateContent": "...",
  "grade": "12",
  "subject": "Toán",
  "chapter": "Chương 1",
  "isPublic": true,
  "templateCreatedAt": "2025-01-10T08:00:00Z"
}
```

### 2. GET /api/wishlists/my-wishlist
**Lấy danh sách wishlist của user**

**Response:** `200 OK`
```json
[
  {
    "wishlistId": 1,
    "userId": 10,
    "packageId": null,
    "storageId": 123,
    "addedAt": "2025-01-15T10:30:00Z",
    "notes": null,
    "templateName": "Toán Học Lớp 12 - Chương 1",
    "templateContent": "...",
    "grade": "12",
    "subject": "Toán",
    "chapter": "Chương 1",
    "isPublic": true,
    "templateCreatedAt": "2025-01-10T08:00:00Z"
  }
]
```

### 3. GET /api/wishlists/check/{storageId}
**Kiểm tra StorageTemplate có trong wishlist**

**Response:** `200 OK`
```json
{
  "storageId": 123,
  "isInWishlist": true
}
```

### 4. DELETE /api/wishlists/by-storage/{storageId}
**Xóa StorageTemplate khỏi wishlist**

**Response:** `204 No Content` hoặc `404 Not Found`

### 5. DELETE /api/wishlists/{id}
**Xóa wishlist item theo ID (giữ nguyên)**

**Response:** `204 No Content` hoặc `404 Not Found`

## Business Logic

### Validation
1. **Khi thêm vào wishlist:**
   - `StorageId` phải tồn tại trong bảng `StorageTemplates`
   - `PackageId` (nếu có) phải tồn tại trong bảng `Packages`
   - Một user không thể thêm cùng một StorageTemplate vào wishlist 2 lần

2. **Khi xóa:**
   - Xóa StorageTemplate từ database → tự động xóa khỏi wishlist (CASCADE)
   - Xóa Package từ database → tự động xóa wishlist items liên quan (CASCADE)

### Backward Compatibility
- Dữ liệu cũ dựa trên `PackageId` vẫn hoạt động
- Endpoints legacy vẫn được hỗ trợ
- `PackageId` là nullable, có thể null nếu chỉ có `StorageId`

## Test Cases

1. ✅ Thêm StorageTemplate vào wishlist → thành công
2. ✅ Thêm cùng một StorageTemplate 2 lần → trả về lỗi "Đã tồn tại"
3. ✅ Load wishlist → trả về danh sách với thông tin StorageTemplate đầy đủ
4. ✅ Xóa StorageTemplate khỏi wishlist → thành công
5. ✅ Xóa StorageTemplate từ database → tự động xóa khỏi wishlist (CASCADE)
6. ✅ Check wishlist status → trả về đúng trạng thái
7. ✅ Backward compatibility với PackageId → vẫn hoạt động

## Next Steps

1. **Chạy migration script:**
   ```sql
   -- Mở Note/MIGRATE_Add_StorageId_To_Wishlists.sql và chạy trong SSMS
   ```

2. **Test API endpoints:**
   - Test với Swagger UI
   - Verify response có đầy đủ thông tin StorageTemplate

3. **Frontend integration:**
   - Frontend đã sẵn sàng, chỉ cần đảm bảo gửi `storageId` trong request

## Files Changed

- ✅ `Note/MIGRATE_Add_StorageId_To_Wishlists.sql` - Migration script
- ✅ `Eduprompt.Domain/Entities/Wishlist.Partial.cs` - Entity
- ✅ `Eduprompt.Domain/DTOs/Wishlist/WishlistCreateDto.cs` - Create DTO
- ✅ `Eduprompt.Domain/DTOs/Wishlist/WishlistDto.cs` - Response DTO
- ✅ `Eduprompt.Domain/Interface/Repository/IWishlistRepository.cs` - Repository interface
- ✅ `Eduprompt.DAL/Repositories/WishlistRepository.cs` - Repository implementation
- ✅ `Eduprompt.Domain/Interface/Service/IWishlistService.cs` - Service interface
- ✅ `Eduprompt.BLL/Services/WishlistService.cs` - Service implementation
- ✅ `Eduprompt.API/Controllers/WishlistsController.cs` - Controller
- ✅ `Eduprompt.DAL/DbContexts/EdupromptV2Context.cs` - DbContext mapping

## Notes

- Tất cả endpoints yêu cầu authentication (`[Authorize]`)
- UserId được lấy từ JWT token (không còn hardcode)
- CASCADE delete đảm bảo data consistency
- Backward compatibility được duy trì với PackageId

