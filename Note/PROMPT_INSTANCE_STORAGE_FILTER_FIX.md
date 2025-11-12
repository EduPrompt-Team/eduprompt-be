# Fix: Prompt Instance GetByStorageId - Filter by UserId

**Date:** 2025-11-02  
**Status:** ✅ **COMPLETED**

---

## 🎯 Vấn Đề

Endpoint `/api/prompt-instances/storage/{storageId}` không filter theo `UserId`, trả về **TẤT CẢ** instances có `PackageId` matching, không chỉ của user hiện tại.

**Ví dụ:**
- User 1 có 4 instances với PackageId = 4
- User 2 có 2 instances với PackageId = 4
- Khi gọi `/api/prompt-instances/storage/11` (StorageTemplate có PackageId = 4)
- Endpoint trả về **6 instances** (4 của User 1 + 2 của User 2)
- Frontend chỉ muốn thấy **4 instances của User 1**

---

## ✅ Fix Đã Hoàn Thành

### 1. **Thêm Method Mới: GetByStorageIdAndUserIdAsync**

**File:** `Eduprompt.Domain/Interface/Service/IPromptInstanceService.cs`

**Thêm:**
```csharp
Task<IEnumerable<PromptInstanceDto>> GetByStorageIdAndUserIdAsync(int storageId, int userId);
```

---

### 2. **Implement Method trong Service**

**File:** `Eduprompt.BLL/Services/PromptInstanceService.cs`

**Thêm:**
```csharp
public async Task<IEnumerable<PromptInstanceDto>> GetByStorageIdAndUserIdAsync(int storageId, int userId)
{
    // Get StorageTemplate to find its PackageId
    var storageTemplate = await _storageTemplateRepository.GetByIdAsync(storageId);
    if (storageTemplate == null)
    {
        return Enumerable.Empty<PromptInstanceDto>();
    }

    // If StorageTemplate has PackageId, get instances by PackageId AND UserId
    if (storageTemplate.PackageId > 0)
    {
        // Get all instances with matching PackageId, then filter by UserId
        var allInstances = await _promptInstanceRepository.GetByTemplateIdAsync(storageTemplate.PackageId);
        var userInstances = allInstances.Where(i => i.UserId == userId);
        return userInstances.Select(MapToDto);
    }

    // If StorageTemplate doesn't have PackageId, return empty (no instances can be linked)
    return Enumerable.Empty<PromptInstanceDto>();
}
```

---

### 3. **Thêm Endpoint Mới: /storage/{storageId}/my**

**File:** `Eduprompt.API/Controllers/PromptInstanceController.cs`

**Thêm:**
```csharp
/// <summary>
/// Lấy instances theo StorageTemplate ID (StorageId) của user hiện tại
/// </summary>
[HttpGet("storage/{storageId}/my")]
public async Task<IActionResult> GetMyInstancesByStorageId(int storageId)
{
    try
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var instances = await _promptInstanceService.GetByStorageIdAndUserIdAsync(storageId, userId);
        return Ok(instances);
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
```

**Clarify endpoint cũ:**
```csharp
/// <summary>
/// Lấy instances theo StorageTemplate ID (StorageId)
/// Note: Returns ALL instances with matching PackageId, not filtered by UserId
/// For user-specific instances, use /storage/{storageId}/my endpoint
/// </summary>
[HttpGet("storage/{storageId}")]
public async Task<IActionResult> GetByStorageId(int storageId)
{
    // ... existing code
}
```

---

## 📋 API Endpoints

### 1. Get Instances by StorageId (All Users)
```
GET /api/prompt-instances/storage/{storageId}
```

**Behavior:**
- Trả về **TẤT CẢ** instances có `PackageId` matching với `StorageTemplate.PackageId`
- **KHÔNG** filter theo `UserId`
- Có thể trả về instances của nhiều users

**Use Case:** Admin hoặc public queries

---

### 2. Get My Instances by StorageId (Current User) - NEW
```
GET /api/prompt-instances/storage/{storageId}/my
```

**Behavior:**
- Trả về instances có `PackageId` matching với `StorageTemplate.PackageId`
- **CHỈ** trả về instances của user hiện tại (authenticated user)
- Tự động lấy `userId` từ JWT token

**Use Case:** User muốn xem instances của chính mình theo template

---

### 3. Get Instances by UserId
```
GET /api/prompt-instances/user/{userId}
```

**Behavior:**
- Trả về **TẤT CẢ** instances của user
- **KHÔNG** filter theo `PackageId` hoặc `StorageId`
- Bao gồm instances với `packageId = null`

**Use Case:** User muốn xem tất cả instances của mình

---

## 🧪 Test Cases

### Test Case 1: Get My Instances by StorageId
```
GET /api/prompt-instances/storage/11/my
User: userId = 1 (from JWT token)

Database:
- StorageTemplate: StorageId=11, PackageId=4
- PromptInstance: InstanceId=8, UserId=1, PackageId=4
- PromptInstance: InstanceId=7, UserId=1, PackageId=4
- PromptInstance: InstanceId=9, UserId=2, PackageId=4 (different user)

Expected Response:
[
  {
    "instanceId": 8,
    "userId": 1,
    "packageId": 4,
    ...
  },
  {
    "instanceId": 7,
    "userId": 1,
    "packageId": 4,
    ...
  }
]
// ✅ Chỉ trả về instances của User 1, không có InstanceId=9
```

### Test Case 2: Get All Instances by StorageId
```
GET /api/prompt-instances/storage/11
User: Any (no filter)

Database:
- StorageTemplate: StorageId=11, PackageId=4
- PromptInstance: InstanceId=8, UserId=1, PackageId=4
- PromptInstance: InstanceId=7, UserId=1, PackageId=4
- PromptInstance: InstanceId=9, UserId=2, PackageId=4

Expected Response:
[
  {
    "instanceId": 8,
    "userId": 1,
    ...
  },
  {
    "instanceId": 7,
    "userId": 1,
    ...
  },
  {
    "instanceId": 9,
    "userId": 2,
    ...
  }
]
// ✅ Trả về tất cả instances có PackageId=4 (của cả User 1 và User 2)
```

---

## ✅ Verification Checklist

- [x] Method `GetByStorageIdAndUserIdAsync` đã được thêm vào interface
- [x] Method đã được implement trong service
- [x] Endpoint `/storage/{storageId}/my` đã được thêm
- [x] Endpoint lấy `userId` từ JWT token
- [x] Endpoint filter đúng theo `UserId`
- [x] Endpoint cũ `/storage/{storageId}` vẫn hoạt động (không breaking)

---

## 📝 Notes

1. **Endpoint `/storage/{storageId}`:**
   - Vẫn trả về tất cả instances (không filter theo UserId)
   - Có thể dùng cho admin hoặc public queries
   - Document rõ ràng trong XML comments

2. **Endpoint `/storage/{storageId}/my`:**
   - Trả về chỉ instances của user hiện tại
   - Tự động lấy `userId` từ JWT token
   - Khuyến nghị frontend sử dụng endpoint này

3. **Performance:**
   - Method `GetByStorageIdAndUserIdAsync` query tất cả instances với PackageId matching, sau đó filter trong memory
   - Nếu có nhiều instances, có thể optimize bằng cách thêm method repository mới: `GetByPackageIdAndUserIdAsync`

---

## 🚀 Next Steps

1. **Frontend Migration:**
   - Thay đổi từ `/api/prompt-instances/storage/{storageId}` sang `/api/prompt-instances/storage/{storageId}/my`
   - Hoặc tiếp tục dùng `/api/prompt-instances/user/{userId}` và filter ở frontend

2. **Optional Optimization:**
   - Thêm method repository: `GetByPackageIdAndUserIdAsync` để query trực tiếp từ database
   - Giảm memory usage khi có nhiều instances

---

**Status:** ✅ **COMPLETED** - Ready for testing!

