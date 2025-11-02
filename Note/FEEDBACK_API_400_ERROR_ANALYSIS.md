# 🚨 FEEDBACK API 400 ERROR - PHÂN TÍCH & GIẢI PHÁP

**Date:** 2025-01-17  
**Status:** ⚠️ **CẦN FIX**

---

## 🔍 ROOT CAUSE ANALYSIS

### **Vấn đề 1: UserId Required trong Request Body** ❌

**Current Code:**
```csharp
// CreateFeedbackDto.cs
[Required]
public int UserId { get; set; }  // ← Frontend KHÔNG gửi field này
```

**Frontend Request:**
```json
{
  "postId": 5,
  "comment": "ádasdasdasd",
  "rating": 4
  // ❌ Thiếu "userId"
}
```

**Error:** Validation fail → 400 Bad Request

---

### **Vấn đề 2: Foreign Key Constraint - PostId không tồn tại** ❌

**Database Schema:**
```sql
CONSTRAINT [FK_Feedbacks_Posts] FOREIGN KEY ([PostID]) REFERENCES [dbo].[Posts]([PostID])
```

**Frontend Request:**
```json
{
  "postId": 5  // ← Là storageId của StorageTemplate, KHÔNG phải PostId
}
```

**Error:** Foreign key constraint violation → 400 Bad Request (hoặc database error)

**Reason:** 
- Frontend gửi `storageId = 5` làm `postId`
- StorageTemplate với `storageId = 5` KHÔNG tồn tại trong `Posts` table
- Foreign key constraint fail khi insert vào `Feedbacks` table

---

### **Vấn đề 3: Controller không lấy UserId từ JWT** ❌

**Current Code:**
```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateFeedbackDto createDto)
{
    // ❌ Không lấy userId từ JWT token
    var feedback = await _feedbackService.CreateAsync(createDto);
    return CreatedAtAction(...);
}
```

**Expected:**
```csharp
var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
createDto.UserId = userId;  // Set từ token
```

---

## ✅ GIẢI PHÁP

### **Option A: Support StorageId trong Feedback (RECOMMENDED)**

**Thay đổi:**

1. **Thêm StorageId vào Feedback Entity**
   ```sql
   ALTER TABLE Feedbacks ADD StorageId INT NULL;
   ALTER TABLE Feedbacks ADD CONSTRAINT FK_Feedbacks_StorageTemplates 
       FOREIGN KEY (StorageId) REFERENCES StorageTemplates(StorageID);
   ```

2. **Update Feedback Entity**
   ```csharp
   public int? StorageId { get; set; }
   public virtual StorageTemplate StorageTemplate { get; set; }
   ```

3. **Update CreateFeedbackDto**
   ```csharp
   public int? PostId { get; set; }      // Optional
   public int? StorageId { get; set; }   // Optional (NEW)
   // UserId - Remove [Required], get from token
   ```

4. **Update Controller**
   ```csharp
   [HttpPost]
   public async Task<IActionResult> Create([FromBody] CreateFeedbackDto createDto)
   {
       // Lấy userId từ JWT token
       var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
       createDto.UserId = userId;
       
       // Support cả PostId và StorageId
       if (createDto.PostId.HasValue && createDto.PostId.Value == 0)
       {
           createDto.PostId = null;  // Clear nếu frontend gửi 0
       }
       
       var feedback = await _feedbackService.CreateAsync(createDto);
       return CreatedAtAction(...);
   }
   ```

5. **Update Service**
   ```csharp
   public async Task<FeedbackDto> CreateAsync(CreateFeedbackDto createDto)
   {
       // Validate: Phải có PostId HOẶC StorageId
       if (!createDto.PostId.HasValue && !createDto.StorageId.HasValue)
       {
           throw new InvalidOperationException("PostId or StorageId is required");
       }
       
       if (createDto.PostId.HasValue)
       {
           var post = await _postRepository.GetByIdAsync(createDto.PostId.Value);
           if (post == null)
               throw new InvalidOperationException($"Post with ID {createDto.PostId} not found");
       }
       
       if (createDto.StorageId.HasValue)
       {
           var storage = await _storageTemplateRepository.GetByIdAsync(createDto.StorageId.Value);
           if (storage == null)
               throw new InvalidOperationException($"StorageTemplate with ID {createDto.StorageId} not found");
       }
       
       var feedback = new Feedback
       {
           PostId = createDto.PostId ?? 0,  // Set 0 if null (for foreign key)
           StorageId = createDto.StorageId,
           UserId = createDto.UserId,
           Rating = createDto.Rating,
           Comment = createDto.Comment,
           CreatedDate = DateTime.UtcNow,
           Status = "Active"
       };
       
       return await _feedbackRepository.CreateAsync(feedback);
   }
   ```

**Ưu điểm:**
- ✅ Support cả Post và StorageTemplate
- ✅ Rõ ràng, không conflict
- ✅ Foreign key constraint hợp lệ

**Nhược điểm:**
- ⚠️ Cần database migration
- ⚠️ PostId phải nullable hoặc set default value

---

### **Option B: Cho phép PostId = 0 nếu là StorageId (QUICK FIX)**

**Thay đổi:**

1. **Update Feedback Entity - Make PostId nullable**
   ```sql
   ALTER TABLE Feedbacks ALTER COLUMN PostID INT NULL;
   ```

2. **Update Service để support StorageId**
   ```csharp
   // Nếu PostId = 0 hoặc null, coi như là StorageId
   if (createDto.PostId == 0 || !createDto.PostId.HasValue)
   {
       // Validate StorageId exists
       var storage = await _storageTemplateRepository.GetByIdAsync(storageId);
       // Use StorageId logic
   }
   ```

**Ưu điểm:**
- ✅ Fix nhanh, không cần nhiều thay đổi
- ✅ Backward compatible

**Nhược điểm:**
- ⚠️ Hacky solution
- ⚠️ PostId = 0 không có ý nghĩa thực sự
- ⚠️ Foreign key constraint vẫn có vấn đề

---

### **Option C: Frontend tạo Post trước (NOT RECOMMENDED)**

Frontend tự động tạo Post cho mỗi StorageTemplate trước khi tạo Feedback.

**Không recommend** vì phức tạp và không cần thiết.

---

## 🎯 RECOMMENDED FIX

**Sử dụng Option A** - Thêm StorageId field vào Feedback.

**Steps:**
1. Database migration - thêm StorageId column
2. Update Feedback entity
3. Update DTOs - support cả PostId và StorageId
4. Update Controller - lấy userId từ token
5. Update Service - validate và handle cả 2 cases
6. Update validators

---

**Updated:** 2025-01-17  
**Priority:** 🔴 **HIGH**

