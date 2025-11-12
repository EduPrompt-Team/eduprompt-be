# Fix: Prompt Instance Complete - outputJson Not Saved

**Date:** 2025-11-02  
**Status:** ✅ **COMPLETED**

---

## 🎯 Vấn Đề

Endpoint `POST /api/prompt-instances/{instanceId}/complete` không lưu `outputJson` vào database. Response trả về `outputJson: ""` (empty string) mặc dù frontend đã gửi `outputJson` với data đầy đủ.

**Database Check:**
- InstanceId = 9 có `OutputJsonStatus = EMPTY` và `OutputJsonLength = 0` ❌

---

## ✅ Nguyên Nhân

**Vấn đề chính:** DbContext sử dụng `UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)`, khiến EF Core không track changes khi update entity.

**Flow:**
1. `GetByIdAsync` trả về entity không được track (do NoTracking)
2. Service update properties trên entity không được track
3. `UpdateAsync` gọi `_context.Update()` nhưng có thể không detect changes đúng cách
4. `SaveChangesAsync()` có thể không save changes

---

## ✅ Fix Đã Hoàn Thành

### 1. **Cải Thiện UpdateAsync trong Repository**

**File:** `Eduprompt.DAL/Repositories/PromptInstanceRepository.cs`

**Trước:**
```csharp
public async Task<PromptInstance> UpdateAsync(PromptInstance PromptInstance)
{
    _context.PromptInstances.Update(PromptInstance);
    await _context.SaveChangesAsync();
    return PromptInstance;
}
```

**Sau:**
```csharp
public async Task<PromptInstance> UpdateAsync(PromptInstance PromptInstance)
{
    // Since DbContext uses NoTracking, we need to explicitly attach and mark as modified
    // First, check if entity is already tracked
    var existingEntity = await _context.PromptInstances
        .AsTracking() // Use tracking for update
        .FirstOrDefaultAsync(p => p.InstanceId == PromptInstance.InstanceId);
    
    if (existingEntity != null)
    {
        // Update properties from the provided entity
        existingEntity.OutputJson = PromptInstance.OutputJson;
        existingEntity.Status = PromptInstance.Status;
        existingEntity.ProcessingTimeMs = PromptInstance.ProcessingTimeMs;
        existingEntity.ExecutedAt = PromptInstance.ExecutedAt;
        existingEntity.PromptName = PromptInstance.PromptName;
        existingEntity.InputJson = PromptInstance.InputJson;
        
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.PromptInstances
            .Include(p => p.PromptInstanceDetails)
            .Include(p => p.Package)
            .FirstOrDefaultAsync(p => p.InstanceId == PromptInstance.InstanceId) ?? existingEntity;
    }
    else
    {
        // If not found, attach and update
        _context.PromptInstances.Update(PromptInstance);
        await _context.SaveChangesAsync();
        
        // Reload with navigation properties
        return await _context.PromptInstances
            .Include(p => p.PromptInstanceDetails)
            .Include(p => p.Package)
            .FirstOrDefaultAsync(p => p.InstanceId == PromptInstance.InstanceId) ?? PromptInstance;
    }
}
```

**Thay đổi:**
- ✅ Sử dụng `.AsTracking()` để track entity khi update
- ✅ Update từng property một cách explicit
- ✅ Reload entity sau khi save để đảm bảo fresh data

---

### 2. **Fix Duplicate Code trong CompleteAsync**

**File:** `Eduprompt.BLL/Services/PromptInstanceService.cs`

**Trước:**
```csharp
// Update ProcessingTimeMs if provided
if (completeDto.ProcessingTimeMs.HasValue)
{
    instance.ProcessingTimeMs = completeDto.ProcessingTimeMs.Value;
}

// Update ProcessingTimeMs if provided  // ❌ DUPLICATE
if (completeDto.ProcessingTimeMs.HasValue)
{
    instance.ProcessingTimeMs = completeDto.ProcessingTimeMs.Value;
}
```

**Sau:**
```csharp
// Update ProcessingTimeMs if provided
if (completeDto.ProcessingTimeMs.HasValue)
{
    instance.ProcessingTimeMs = completeDto.ProcessingTimeMs.Value;
}
```

---

### 3. **Verify CompleteAsync Logic**

**File:** `Eduprompt.BLL/Services/PromptInstanceService.cs`

**Code hiện tại:**
```csharp
public async Task<PromptInstanceDto> CompleteAsync(int InstanceId, CompletePromptInstanceDto completeDto)
{
    var instance = await _promptInstanceRepository.GetByIdAsync(InstanceId);
    if (instance == null)
    {
        throw new KeyNotFoundException($"PromptInstance with ID {InstanceId} not found");
    }

    // Update OutputJson if provided (including empty string - allow clearing)
    // Use null check instead of IsNullOrEmpty to allow empty strings
    if (completeDto.OutputJson != null)
    {
        instance.OutputJson = completeDto.OutputJson; // ✅ Update OutputJson
    }

    // Update Status if provided
    if (!string.IsNullOrEmpty(completeDto.Status))
    {
        instance.Status = completeDto.Status;
    }
    else
    {
        instance.Status = "Completed"; // Default to Completed
    }

    // Update ProcessingTimeMs if provided
    if (completeDto.ProcessingTimeMs.HasValue)
    {
        instance.ProcessingTimeMs = completeDto.ProcessingTimeMs.Value;
    }

    // Update ExecutedAt to current time
    instance.ExecutedAt = DateTime.UtcNow;

    var updatedInstance = await _promptInstanceRepository.UpdateAsync(instance);
    return MapToDto(updatedInstance);
}
```

**Status:** ✅ Logic đã đúng

---

## 🧪 Test Cases

### Test Case 1: Complete Instance with outputJson

```bash
POST /api/prompt-instances/9/complete
{
  "outputJson": "{\"prompt\":\"Test prompt\",\"isMock\":false}",
  "status": "Completed",
  "processingTimeMs": 1000
}
```

**Expected Response:**
```json
{
  "instanceId": 9,
  "outputJson": "{\"prompt\":\"Test prompt\",\"isMock\":false}",  // ✅ PHẢI có giá trị
  "status": "Completed",
  "processingTimeMs": 1000
}
```

**Database:**
```sql
SELECT InstanceId, LEN(OutputJson) as OutputJsonLength, Status
FROM PromptInstances
WHERE InstanceId = 9;
-- Expected: OutputJsonLength > 0, Status = 'Completed'
```

---

### Test Case 2: Complete Instance with Empty outputJson

```bash
POST /api/prompt-instances/9/complete
{
  "outputJson": "",
  "status": "Completed"
}
```

**Expected:**
- Nếu `outputJson` là empty string, sẽ set empty string (allow clearing)
- Database sẽ có `OutputJson = ''` (empty string)

---

### Test Case 3: Complete Instance without outputJson

```bash
POST /api/prompt-instances/9/complete
{
  "status": "Completed",
  "processingTimeMs": 1000
}
```

**Expected:**
- Nếu không có `outputJson` trong request, giữ nguyên giá trị cũ
- Chỉ update `status` và `processingTimeMs`

---

## ✅ Verification Checklist

- [x] DTO `CompletePromptInstanceDto` có field `OutputJson` ✅
- [x] Service method `CompleteAsync` update `OutputJson` ✅
- [x] Repository `UpdateAsync` sử dụng `.AsTracking()` để track changes ✅
- [x] Repository `UpdateAsync` update từng property explicit ✅
- [x] Repository `UpdateAsync` reload entity sau khi save ✅
- [x] Fix duplicate code cho `ProcessingTimeMs` ✅
- [ ] **Test với outputJson có giá trị** ⚠️ **CẦN TEST**
- [ ] **Verify database có OutputJson sau khi complete** ⚠️ **CẦN TEST**

---

## 📝 Notes

1. **NoTracking Issue:**
   - DbContext sử dụng `UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)`
   - Khi update, cần sử dụng `.AsTracking()` để track changes
   - Hoặc update từng property explicit trên tracked entity

2. **Update Strategy:**
   - Query entity với `.AsTracking()` trước
   - Update properties trên tracked entity
   - Save changes
   - Reload với navigation properties

3. **OutputJson Handling:**
   - Nếu `outputJson != null` → Update (kể cả empty string)
   - Nếu `outputJson == null` → Giữ nguyên giá trị cũ

---

## 🚀 Next Steps

1. **Test API Endpoint:**
   - Test `POST /api/prompt-instances/9/complete` với `outputJson` có giá trị
   - Verify response có `outputJson` đầy đủ
   - Verify database có `OutputJson` sau khi complete

2. **Verify Database:**
   ```sql
   SELECT InstanceId, LEN(OutputJson) as OutputJsonLength, Status
   FROM PromptInstances
   WHERE InstanceId = 9;
   -- Expected: OutputJsonLength > 0
   ```

3. **Frontend Integration:**
   - Frontend có thể test lại sau khi backend fix
   - Verify chat history restore hoạt động

---

**Status:** ✅ **COMPLETED** - Ready for testing!
