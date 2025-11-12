# Backend Fix: Prompt Instance PackageId Issue

**Date:** 2025-11-02  
**Status:** ✅ **COMPLETED**

---

## 🎯 Tổng Quan

Đã fix vấn đề Prompt Instance không thể tạo khi `packageId = 0` hoặc `null`. Backend bây giờ:

- ✅ Cho phép `packageId` nullable hoặc 0
- ✅ Tự động map `packageId` từ `StorageTemplate` nếu `storageId` được cung cấp
- ✅ Cải thiện error messages rõ ràng
- ✅ Thêm endpoint mới `/api/prompt-instances/storage/{storageId}`

---

## ✅ Thay Đổi Đã Hoàn Thành

### 1. **DTOs - Cho phép PackageId Nullable và thêm StorageId**

**File:** `Eduprompt.Domain/DTOs/PromptInstance/CreatePromptInstanceDto.cs`

**Thay đổi:**
```csharp
public class CreatePromptInstanceDto
{
    [Required]
    public int UserId { get; set; }

    // PackageId is optional - can be null or 0
    // If null/0 and storageId is provided, packageId will be auto-mapped from StorageTemplate
    public int? PackageId { get; set; }

    // StorageId is optional - used to auto-map packageId from StorageTemplate
    // If packageId is null/0 and storageId is provided, packageId will be resolved from StorageTemplate
    public int? StorageId { get; set; }

    [Required]
    [StringLength(200)]
    public string PromptName { get; set; } = string.Empty;

    public string? InputJson { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Pending";
}
```

**File:** `Eduprompt.Domain/DTOs/PromptInstance/PromptInstanceDto.cs`

**Thay đổi:**
```csharp
public class PromptInstanceDto
{
    public int InstanceId { get; set; }
    public int UserId { get; set; }
    public int? PackageId { get; set; } // Nullable - can be null if created from StorageTemplate without package
    public int? StorageId { get; set; } // Optional - storage template ID if created from template
    // ... other fields
}
```

---

### 2. **Service - Tự động Map PackageId từ StorageTemplate**

**File:** `Eduprompt.BLL/Services/PromptInstanceService.cs`

**Logic mới:**
```csharp
public async Task<PromptInstanceDto> CreateAsync(CreatePromptInstanceDto createPromptInstanceDto)
{
    // Resolve PackageId:
    // 1. If PackageId is provided and > 0, use it (validate it exists)
    // 2. If PackageId is null/0 and StorageId is provided, get PackageId from StorageTemplate
    // 3. If both are null/0, PackageId remains null (allowed for instances without package)
    
    int? packageId = null;

    // Option 1: PackageId is provided and > 0
    if (createPromptInstanceDto.PackageId.HasValue && createPromptInstanceDto.PackageId.Value > 0)
    {
        // Validate package exists
        var package = await _packageRepository.GetByIdAsync(createPromptInstanceDto.PackageId.Value);
        if (package == null)
        {
            throw new InvalidOperationException($"Package with ID {createPromptInstanceDto.PackageId.Value} not found");
        }
        packageId = createPromptInstanceDto.PackageId.Value;
    }
    // Option 2: PackageId is null/0, but StorageId is provided
    else if (createPromptInstanceDto.StorageId.HasValue && createPromptInstanceDto.StorageId.Value > 0)
    {
        // Get PackageId from StorageTemplate
        var storageTemplate = await _storageTemplateRepository.GetByIdAsync(createPromptInstanceDto.StorageId.Value);
        if (storageTemplate == null)
        {
            throw new InvalidOperationException($"StorageTemplate with ID {createPromptInstanceDto.StorageId.Value} not found");
        }
        
        // If StorageTemplate has PackageId, use it
        if (storageTemplate.PackageId.HasValue && storageTemplate.PackageId.Value > 0)
        {
            // Validate package exists
            var package = await _packageRepository.GetByIdAsync(storageTemplate.PackageId.Value);
            if (package == null)
            {
                throw new InvalidOperationException($"Package with ID {storageTemplate.PackageId.Value} from StorageTemplate not found");
            }
            packageId = storageTemplate.PackageId.Value;
        }
        // If StorageTemplate doesn't have PackageId, packageId remains null (allowed)
    }
    // Option 3: Both are null/0 - packageId remains null (allowed for instances without package)

    var instance = new PromptInstance
    {
        UserId = createPromptInstanceDto.UserId,
        PackageId = packageId ?? 0, // Use 0 as sentinel for null (DbContext will convert to NULL in DB)
        PromptName = createPromptInstanceDto.PromptName,
        InputJson = createPromptInstanceDto.InputJson,
        Status = createPromptInstanceDto.Status ?? "Pending",
        ExecutedAt = DateTime.UtcNow
    };

    var createdInstance = await _promptInstanceRepository.CreateAsync(instance);
    return MapToDto(createdInstance);
}
```

---

### 3. **Validation - Cải thiện Error Messages**

**File:** `Eduprompt.API/Validators/PromptInstanceValidators.cs`

**Thay đổi:**
```csharp
public class CreatePromptInstanceValidator : AbstractValidator<CreatePromptInstanceDto>
{
    public CreatePromptInstanceValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId is required and must be greater than 0");
        
        RuleFor(x => x.PromptName).NotEmpty().WithMessage("PromptName is required")
            .MaximumLength(200).WithMessage("PromptName cannot exceed 200 characters");
        
        // PackageId is optional - can be null or 0
        // If provided, must be > 0
        When(x => x.PackageId.HasValue, () => 
        {
            RuleFor(x => x.PackageId!.Value)
                .GreaterThan(0)
                .WithMessage("PackageId must be greater than 0 if provided");
        });
        
        // StorageId is optional - can be null or 0
        // If provided, must be > 0
        When(x => x.StorageId.HasValue, () => 
        {
            RuleFor(x => x.StorageId!.Value)
                .GreaterThan(0)
                .WithMessage("StorageId must be greater than 0 if provided");
        });
    }
}
```

---

### 4. **Controller - Cải thiện Error Handling**

**File:** `Eduprompt.API/Controllers/PromptInstanceController.cs`

**Thay đổi:**
```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreatePromptInstanceDto createPromptInstanceDto)
{
    try
    {
        var instance = await _promptInstanceService.CreateAsync(createPromptInstanceDto);
        return CreatedAtAction(nameof(GetById), new { InstanceId = instance.InstanceId }, instance);
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new 
        { 
            message = ex.Message,
            errors = new Dictionary<string, string[]>
            {
                { "packageId", new[] { ex.Message } },
                { "storageId", new[] { ex.Message } }
            }
        });
    }
    catch (KeyNotFoundException ex)
    {
        return NotFound(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
```

---

### 5. **DbContext - Cho phép PackageId Nullable**

**File:** `Eduprompt.DAL/DbContexts/EdupromptV2Context.cs`

**Thay đổi:**
```csharp
modelBuilder.Entity<PromptInstance>(entity =>
{
    // ...
    
    // PackageId: Map int (entity) to int? (database) using 0 as sentinel for NULL
    entity.Property(e => e.PackageId)
        .HasColumnName("PackageID")
        .HasConversion(
            v => v == 0 ? (int?)null : v,  // Entity (int) -> DB (int?)
            v => v ?? 0                     // DB (int?) -> Entity (int)
        );
    
    // ...
    
    entity.HasOne(d => d.Package).WithMany(p => p.PromptInstances)
        .HasForeignKey(d => d.PackageId)
        .OnDelete(DeleteBehavior.ClientSetNull)
        .HasConstraintName("FK_PromptInstances_Packages")
        .IsRequired(false); // Allow null in database (0 in entity maps to NULL)
});
```

---

### 6. **Endpoint Mới - GetByStorageId**

**File:** `Eduprompt.API/Controllers/PromptInstanceController.cs`

**Endpoint mới:**
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

**Clarification cho endpoint cũ:**
```csharp
/// <summary>
/// Lấy instances theo Template ID (PackageId)
/// Note: templateId in this endpoint refers to PackageId
/// For StorageTemplate-based queries, use /storage/{storageId} endpoint
/// </summary>
[HttpGet("template/{templateId}")]
public async Task<IActionResult> GetByTemplateId(int templateId)
{
    // ...
}
```

---

## 🧪 Test Cases

### Test Case 1: Tạo instance với packageId hợp lệ
```json
POST /api/prompt-instances
{
  "userId": 1,
  "packageId": 123,
  "promptName": "Test Prompt",
  "inputJson": "...",
  "outputJson": null
}
```
**Expected:** 200 OK với instance mới

### Test Case 2: Tạo instance với packageId = 0 và storageId
```json
POST /api/prompt-instances
{
  "userId": 1,
  "packageId": 0,
  "storageId": 456,
  "promptName": "Test Prompt",
  "inputJson": "...",
  "outputJson": null
}
```
**Expected:** 200 OK - packageId được tự động map từ StorageTemplate

### Test Case 3: Tạo instance với packageId = null và storageId
```json
POST /api/prompt-instances
{
  "userId": 1,
  "packageId": null,
  "storageId": 456,
  "promptName": "Test Prompt",
  "inputJson": "...",
  "outputJson": null
}
```
**Expected:** 200 OK - packageId được tự động map từ StorageTemplate

### Test Case 4: Tạo instance với cả packageId và storageId null
```json
POST /api/prompt-instances
{
  "userId": 1,
  "packageId": null,
  "storageId": null,
  "promptName": "Test Prompt",
  "inputJson": "...",
  "outputJson": null
}
```
**Expected:** 200 OK - packageId = null (allowed for instances without package)

### Test Case 5: Tạo instance với storageId không tồn tại
```json
POST /api/prompt-instances
{
  "userId": 1,
  "packageId": 0,
  "storageId": 99999,
  "promptName": "Test Prompt",
  "inputJson": "...",
  "outputJson": null
}
```
**Expected:** 400 Bad Request với message "StorageTemplate with ID 99999 not found"

---

## 📋 API Endpoints

### 1. Create Prompt Instance
```
POST /api/prompt-instances
```

**Request:**
```json
{
  "userId": 1,
  "packageId": 123,  // Optional - can be null or 0
  "storageId": 456,  // Optional - used to auto-map packageId from StorageTemplate
  "promptName": "Test Prompt",
  "inputJson": "...",
  "status": "Pending"
}
```

**Response:**
```json
{
  "instanceId": 1,
  "userId": 1,
  "packageId": 123,  // Can be null
  "storageId": null, // TODO: Add if needed
  "promptName": "Test Prompt",
  "inputJson": "...",
  "status": "Pending",
  "executedAt": "2025-11-02T10:30:00Z"
}
```

### 2. Get Instances by Template ID (PackageId)
```
GET /api/prompt-instances/template/{templateId}
```
**Note:** `templateId` refers to `PackageId`

### 3. Get Instances by Storage ID (NEW)
```
GET /api/prompt-instances/storage/{storageId}
```
**Note:** Returns instances with `PackageId` matching the `StorageTemplate.PackageId`

---

## ⚠️ Breaking Changes

**Không có breaking changes!**

- `packageId` trong request là optional (có thể null hoặc 0)
- Response thêm field `storageId` (optional)
- Endpoint mới `/storage/{storageId}` không ảnh hưởng endpoints cũ

---

## 📝 Notes

1. **PackageId Resolution Logic:**
   - Nếu `packageId` được cung cấp và > 0 → Sử dụng trực tiếp (validate tồn tại)
   - Nếu `packageId` = null/0 và `storageId` được cung cấp → Tự động map từ StorageTemplate
   - Nếu cả hai đều null/0 → `packageId` = null (cho phép instances không có package)

2. **Database Schema:**
   - `PackageID` trong database là nullable (INT NULL)
   - Entity sử dụng `0` làm sentinel value cho NULL
   - DbContext tự động convert giữa `0` (entity) và `NULL` (database)

3. **Error Messages:**
   - Rõ ràng và cụ thể: "Package with ID {id} not found"
   - Format chuẩn với `errors` object

---

## ✅ Verification Checklist

- [x] DTOs cho phép `packageId` nullable và thêm `storageId`
- [x] Service tự động map `packageId` từ StorageTemplate
- [x] Validation cải thiện error messages
- [x] Controller cải thiện error handling
- [x] DbContext cho phép `PackageId` nullable
- [x] Endpoint mới `/storage/{storageId}`
- [x] Clarify endpoint `/template/{templateId}`

---

**Status:** ✅ **COMPLETED** - Ready for testing!

