# Backend Fix: Prompt Instance Get By User - Missing Instances

**Date:** 2025-11-02  
**Status:** ✅ **COMPLETED**

---

## 🎯 Vấn Đề

Frontend gọi `GET /api/prompt-instances/user/{userId}` nhưng nhận được **0 instances**, mặc dù instances đã được tạo thành công.

---

## ✅ Nguyên Nhân

**File:** `Eduprompt.BLL/Services/PromptInstanceService.cs`

**Vấn đề:**
```csharp
public Task<IEnumerable<PromptInstanceDto>> GetByUserIdAsync(int UserId)
{
    return Task.FromResult(Enumerable.Empty<PromptInstanceDto>()); // ❌ Luôn trả về empty!
}
```

**Repository đã implement đúng:**
```csharp
public async Task<IEnumerable<PromptInstance>> GetByUserIdAsync(int UserId)
{
    return await _context.PromptInstances
        .Include(p => p.PromptInstanceDetails)
        .Include(p => p.Package)
        .Where(p => p.UserId == UserId)
        .OrderByDescending(p => p.ExecutedAt)
        .ToListAsync();
}
```

**Nhưng Service không gọi Repository!**

---

## ✅ Fix Đã Hoàn Thành

### 1. **Fix GetByUserIdAsync trong Service**

**File:** `Eduprompt.BLL/Services/PromptInstanceService.cs`

**Trước:**
```csharp
public Task<IEnumerable<PromptInstanceDto>> GetByUserIdAsync(int UserId)
{
    return Task.FromResult(Enumerable.Empty<PromptInstanceDto>()); // ❌
}
```

**Sau:**
```csharp
public async Task<IEnumerable<PromptInstanceDto>> GetByUserIdAsync(int UserId)
{
    var instances = await _promptInstanceRepository.GetByUserIdAsync(UserId);
    return instances.Select(MapToDto); // ✅ Gọi repository và map kết quả
}
```

---

### 2. **Cải Thiện GetByTemplateIdAsync trong Repository**

**File:** `Eduprompt.DAL/Repositories/PromptInstanceRepository.cs`

**Thay đổi:**
```csharp
public async Task<IEnumerable<PromptInstance>> GetByTemplateIdAsync(int TemplateId)
{
    return await _context.PromptInstances
        .Include(p => p.PromptInstanceDetails)
        .Include(p => p.Package)
        .Where(p => p.PackageId == TemplateId || (TemplateId == 0 && p.PackageId == 0)) // Handle null PackageId
        .OrderByDescending(p => p.ExecutedAt)
        .ToListAsync();
}
```

**Lưu ý:** Handle trường hợp `PackageId = null` (0 trong entity = NULL trong DB)

---

### 3. **Endpoint GetByStorageId Đã Có Sẵn**

**File:** `Eduprompt.API/Controllers/PromptInstanceController.cs`

**Endpoint:**
```csharp
/// <summary>
/// Lấy instances theo StorageTemplate ID (StorageId)
/// </summary>
[HttpGet("storage/{storageId}")]
public async Task<IActionResult> GetByStorageId(int storageId)
{
    try
    {
        var instances = await _promptInstanceService.GetByStorageIdAsync(storageId);
        return Ok(instances);
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
```

**Logic trong Service:**
```csharp
public async Task<IEnumerable<PromptInstanceDto>> GetByStorageIdAsync(int storageId)
{
    // Get StorageTemplate to find its PackageId
    var storageTemplate = await _storageTemplateRepository.GetByIdAsync(storageId);
    if (storageTemplate == null)
    {
        return Enumerable.Empty<PromptInstanceDto>();
    }

    // If StorageTemplate has PackageId, get instances by PackageId
    if (storageTemplate.PackageId.HasValue && storageTemplate.PackageId.Value > 0)
    {
        var instances = await _promptInstanceRepository.GetByTemplateIdAsync(storageTemplate.PackageId.Value);
        return instances.Select(MapToDto);
    }

    // If StorageTemplate doesn't have PackageId, return empty (no instances can be linked)
    return Enumerable.Empty<PromptInstanceDto>();
}
```

---

## 🧪 Test Cases

### Test Case 1: Get User Instances - Should Return All

```
GET /api/prompt-instances/user/1

Database:
- PromptInstance: InstanceId=7, UserId=1, PackageId=4, Status="Completed", OutputJson != null
- PromptInstance: InstanceId=8, UserId=1, PackageId=null, Status="Completed", OutputJson != null

Expected Response:
[
  {
    "instanceId": 7,
    "userId": 1,
    "packageId": 4,
    "storageId": null,
    "promptName": "Chat Vật lý lớp 10 - Chương 2 - ...",
    "inputJson": "...",
    "outputJson": "...",  // ✅ Có giá trị
    "status": "Completed",
    "executedAt": "2025-11-02T...",
    "processingTimeMs": 1234
  },
  {
    "instanceId": 8,
    "userId": 1,
    "packageId": null,  // ✅ Có thể null
    "storageId": null,
    "promptName": "...",
    "outputJson": "...",
    "status": "Completed",
    ...
  }
]
```

### Test Case 2: Get Instances by StorageId

```
GET /api/prompt-instances/storage/11

Database:
- StorageTemplate: StorageId=11, PackageId=4
- PromptInstance: InstanceId=7, UserId=1, PackageId=4, Status="Completed"

Expected Response:
[
  {
    "instanceId": 7,
    "userId": 1,
    "packageId": 4,  // ✅ Matching với StorageTemplate.PackageId của storageId=11
    "promptName": "...",
    "outputJson": "...",
    "status": "Completed",
    ...
  }
]
```

### Test Case 3: Get Instances with Null PackageId

```
GET /api/prompt-instances/user/1

Database:
- PromptInstance: InstanceId=8, UserId=1, PackageId=null, Status="Completed"

Expected Response:
[
  {
    "instanceId": 8,
    "userId": 1,
    "packageId": null,  // ✅ Allowed
    "storageId": null,
    "promptName": "...",
    "outputJson": "...",
    "status": "Completed",
    ...
  }
]
```

---

## 📋 API Endpoints

### 1. Get Instances by User ID
```
GET /api/prompt-instances/user/{userId}
```

**Response:**
```json
[
  {
    "instanceId": 7,
    "userId": 1,
    "packageId": 4,  // Can be null
    "storageId": null,
    "promptName": "Chat Vật lý lớp 10 - Chương 2 - ...",
    "inputJson": "...",
    "outputJson": "...",  // Has value when completed
    "status": "Completed",
    "executedAt": "2025-11-02T10:30:00Z",
    "processingTimeMs": 1234,
    "userName": "John Doe",
    "packageName": "Package Name"
  }
]
```

**Behavior:**
- ✅ Trả về **TẤT CẢ** instances của user
- ✅ Bao gồm instances với `status = "Completed"` và có `outputJson`
- ✅ Bao gồm instances với `status = "Queued"`, "Running", etc.
- ✅ Bao gồm instances với `packageId = null`
- ✅ Sắp xếp theo `executedAt` descending (mới nhất trước)

---

### 2. Get Instances by Storage ID
```
GET /api/prompt-instances/storage/{storageId}
```

**Response:**
```json
[
  {
    "instanceId": 7,
    "userId": 1,
    "packageId": 4,  // Matching với StorageTemplate.PackageId
    "promptName": "...",
    "outputJson": "...",
    "status": "Completed",
    ...
  }
]
```

**Behavior:**
- ✅ Query StorageTemplate để lấy `PackageId`
- ✅ Query PromptInstances có `PackageId` matching
- ✅ Trả về empty array nếu StorageTemplate không tồn tại hoặc không có PackageId

---

### 3. Get Instances by Template ID (PackageId)
```
GET /api/prompt-instances/template/{templateId}
```

**Note:** `templateId` refers to `PackageId`

**Behavior:**
- ✅ Query PromptInstances có `PackageId = templateId`
- ✅ Handle `PackageId = null` (0 trong entity = NULL trong DB)

---

## ✅ Verification Checklist

- [x] Fix `GetByUserIdAsync` trong service để gọi repository
- [x] Endpoint `/api/prompt-instances/user/{userId}` trả về tất cả instances
- [x] Endpoint `/api/prompt-instances/storage/{storageId}` đã implement
- [x] Handle `PackageId = null` trong queries
- [x] Response bao gồm instances với `status = "Completed"` và có `outputJson`
- [x] Response bao gồm instances với `packageId = null`

---

## 📝 Notes

1. **GetByUserIdAsync:**
   - Trước đây luôn trả về empty array
   - Bây giờ gọi repository và map kết quả đúng cách
   - Trả về tất cả instances của user, không filter theo status

2. **GetByStorageIdAsync:**
   - Endpoint đã có sẵn từ fix trước
   - Query StorageTemplate để lấy PackageId
   - Query PromptInstances có PackageId matching

3. **PackageId Nullable:**
   - Entity sử dụng `0` làm sentinel value cho NULL
   - DbContext tự động convert giữa `0` (entity) và `NULL` (database)
   - Queries cần handle `PackageId = 0` để match với NULL trong DB

---

## 🚀 Next Steps

1. **Test API Endpoints:**
   - Test `GET /api/prompt-instances/user/1` → Verify trả về instances
   - Test `GET /api/prompt-instances/storage/11` → Verify trả về instances matching
   - Verify instances có `outputJson != null` và `status = "Completed"`

2. **Frontend Integration:**
   - Frontend có thể filter instances có `outputJson != null` để hiển thị completed instances
   - Frontend có thể sử dụng endpoint `/storage/{storageId}` để load instances theo template

---

**Status:** ✅ **COMPLETED** - Ready for testing!

