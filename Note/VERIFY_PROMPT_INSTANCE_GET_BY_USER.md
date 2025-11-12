# Verification: Prompt Instance Get By User - Debugging Guide

**Date:** 2025-11-02  
**Status:** 🔍 **DEBUGGING**

---

## ✅ Code Verification

### 1. Service Implementation - ✅ CORRECT

**File:** `Eduprompt.BLL/Services/PromptInstanceService.cs`

```csharp
public async Task<IEnumerable<PromptInstanceDto>> GetByUserIdAsync(int UserId)
{
    var instances = await _promptInstanceRepository.GetByUserIdAsync(UserId);
    return instances.Select(MapToDto); // ✅ CORRECT - Gọi repository
}
```

**Status:** ✅ Code đã được fix đúng

---

### 2. Repository Implementation - ✅ CORRECT

**File:** `Eduprompt.DAL/Repositories/PromptInstanceRepository.cs`

```csharp
public async Task<IEnumerable<PromptInstance>> GetByUserIdAsync(int UserId)
{
    return await _context.PromptInstances
        .Include(p => p.PromptInstanceDetails)
        .Include(p => p.Package)
        .Where(p => p.UserId == UserId) // ✅ CORRECT - Filter đúng
        .OrderByDescending(p => p.ExecutedAt)
        .ToListAsync();
}
```

**Status:** ✅ Code đã đúng

---

### 3. Controller Implementation - ✅ CORRECT

**File:** `Eduprompt.API/Controllers/PromptInstanceController.cs`

```csharp
[HttpGet("user/{UserId}")]
public async Task<IActionResult> GetByUserId(int UserId)
{
    try
    {
        var instances = await _promptInstanceService.GetByUserIdAsync(UserId);
        return Ok(instances); // ✅ CORRECT
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
```

**Status:** ✅ Code đã đúng

---

## ✅ Database Verification

### SQL Query Results:

```sql
SELECT 
    InstanceId, 
    UserId, 
    PackageID, 
    Status, 
    CASE 
        WHEN OutputJson IS NULL THEN 'NULL' 
        ELSE 'HAS VALUE' 
    END as OutputJsonStatus,
    LEN(OutputJson) as OutputJsonLength,
    ExecutedAt 
FROM PromptInstances 
WHERE UserId = 1 
ORDER BY ExecutedAt DESC;
```

**Results:**
```
InstanceId | UserId | PackageID | Status    | OutputJsonStatus | OutputJsonLength | ExecutedAt
-----------|--------|-----------|-----------|------------------|------------------|------------------
8          | 1      | 4         | Completed | HAS VALUE        | 0                | 2025-11-12 21:34:55
7          | 1      | 4         | Completed | HAS VALUE        | 0                | 2025-11-12 21:20:21
6          | 1      | 4         | Completed | HAS VALUE        | 0                | 2025-11-12 21:09:59
5          | 1      | 4         | Completed | HAS VALUE        | 0                | 2025-11-12 21:03:59
```

**Status:** ✅ Database có 4 instances với UserId = 1, bao gồm instanceId = 8

**Note:** `OutputJsonLength = 0` có nghĩa là OutputJson là empty string (`''`), không phải NULL

---

## 🔍 Possible Issues

### Issue 1: Backend Not Restarted

**Problem:** Code đã được fix nhưng backend chưa restart, vẫn chạy code cũ.

**Solution:**
1. Stop backend API
2. Rebuild project: `dotnet build`
3. Start backend API: `dotnet run`
4. Test lại endpoint

---

### Issue 2: DbContext Tracking Issue

**Problem:** `EdupromptV2Context` có `UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)` có thể gây vấn đề.

**Check:**
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection"))
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); // ⚠️ Có thể gây vấn đề
```

**Solution:** 
- `NoTracking` thường không gây vấn đề cho read queries
- Nhưng nếu có vấn đề, có thể thử thêm `.AsTracking()` vào query

---

### Issue 3: PackageId Mapping Issue

**Problem:** PackageId trong database là `4`, nhưng entity có thể map sai do conversion.

**Check:**
```csharp
// DbContext mapping
entity.Property(e => e.PackageId)
    .HasColumnName("PackageID")
    .HasConversion(
        v => v == 0 ? (int?)null : v,  // Entity (int) -> DB (int?)
        v => v ?? 0                     // DB (int?) -> Entity (int)
    );
```

**Status:** ✅ Mapping đúng - PackageId = 4 trong DB sẽ map thành 4 trong entity

---

### Issue 4: OutputJson Empty String

**Problem:** `OutputJsonLength = 0` có nghĩa là OutputJson là empty string, không phải NULL.

**Check:**
```csharp
// MapToDto
OutputJson = promptInstance.OutputJson, // Có thể là empty string ""
```

**Frontend Filter:**
```typescript
const completedInstances = userInstances.filter(inst => inst.outputJson != null)
// ⚠️ Filter này sẽ include empty string, nhưng frontend có thể check length
```

**Solution:** 
- Frontend nên check: `inst.outputJson && inst.outputJson.length > 0`
- Hoặc backend có thể filter: `inst.outputJson != null && inst.outputJson != ""`

---

## 🧪 Debugging Steps

### Step 1: Verify Backend Restarted

```bash
# Stop backend
# Rebuild
dotnet build

# Start backend
dotnet run
```

---

### Step 2: Test API Directly

```bash
# Test endpoint
curl -X GET "https://localhost:7199/api/prompt-instances/user/1" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json"
```

**Expected Response:**
```json
[
  {
    "instanceId": 8,
    "userId": 1,
    "packageId": 4,
    "storageId": null,
    "promptName": "...",
    "inputJson": "...",
    "outputJson": "",  // Empty string (not null)
    "status": "Completed",
    "executedAt": "2025-11-12T21:34:55Z",
    "processingTimeMs": null
  },
  // ... other instances
]
```

---

### Step 3: Add Logging to Service

**File:** `Eduprompt.BLL/Services/PromptInstanceService.cs`

```csharp
public async Task<IEnumerable<PromptInstanceDto>> GetByUserIdAsync(int UserId)
{
    var instances = await _promptInstanceRepository.GetByUserIdAsync(UserId);
    
    // Add logging
    Console.WriteLine($"[PromptInstanceService] GetByUserIdAsync: UserId={UserId}, Count={instances.Count()}");
    foreach (var inst in instances)
    {
        Console.WriteLine($"[PromptInstanceService] Instance: Id={inst.InstanceId}, Status={inst.Status}, OutputJson={(inst.OutputJson != null ? "NOT NULL" : "NULL")}");
    }
    
    return instances.Select(MapToDto);
}
```

---

### Step 4: Check Database Connection

**Verify:** Database connection string đúng và có thể connect.

```sql
-- Test connection
SELECT @@VERSION;

-- Test query
SELECT COUNT(*) FROM PromptInstances WHERE UserId = 1;
```

---

## 📋 Verification Checklist

- [x] Service method `GetByUserIdAsync` gọi repository ✅
- [x] Repository query filter đúng `UserId` ✅
- [x] Instance có trong database với `UserId = 1` ✅
- [x] Instance có `Status = "Completed"` ✅
- [x] Instance có `OutputJson` (empty string, not null) ✅
- [x] Controller endpoint route đúng ✅
- [x] MapToDto map đúng tất cả fields ✅
- [ ] **Backend đã restart sau khi fix** ⚠️ **CẦN KIỂM TRA**
- [ ] **API test trực tiếp trả về instances** ⚠️ **CẦN TEST**

---

## 🚨 Most Likely Issue

**Backend chưa restart sau khi fix code!**

Code đã được fix đúng, database có data, nhưng backend vẫn chạy code cũ (trả về empty array).

**Solution:**
1. **Stop backend API**
2. **Rebuild project:** `dotnet build`
3. **Start backend API:** `dotnet run`
4. **Test endpoint:** `GET /api/prompt-instances/user/1`

---

## 📝 Notes

1. **OutputJson là Empty String:**
   - Database: `OutputJsonLength = 0` → Empty string `''`
   - Frontend filter: `inst.outputJson != null` → Will include empty string
   - Frontend nên check: `inst.outputJson && inst.outputJson.length > 0`

2. **PackageId Mapping:**
   - Database: `PackageID = 4` (INT)
   - Entity: `PackageId = 4` (int)
   - DTO: `PackageId = 4` (int?)
   - Mapping: ✅ Correct

3. **NoTracking:**
   - `UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)` không gây vấn đề cho read queries
   - Nếu có vấn đề, có thể thử thêm `.AsTracking()` vào query

---

## 🔧 Quick Fix Script

**File:** `Note/VERIFY_PROMPT_INSTANCE_DATA.sql`

```sql
-- Verify instances exist
SELECT 
    InstanceId,
    UserId,
    PackageID,
    Status,
    CASE 
        WHEN OutputJson IS NULL THEN 'NULL'
        WHEN OutputJson = '' THEN 'EMPTY'
        ELSE 'HAS VALUE'
    END as OutputJsonStatus,
    LEN(OutputJson) as OutputJsonLength,
    ExecutedAt
FROM PromptInstances 
WHERE UserId = 1
ORDER BY ExecutedAt DESC;

-- Check specific instance
SELECT * FROM PromptInstances WHERE InstanceId = 8;

-- Count instances
SELECT COUNT(*) as TotalInstances 
FROM PromptInstances 
WHERE UserId = 1;
```

---

**Status:** 🔍 **DEBUGGING** - Code đã đúng, cần verify backend restart và test API trực tiếp.

