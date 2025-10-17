# Hướng Dẫn Đồng Bộ Database Với Entity Models

## Vấn Đề

Bạn gặp lỗi **"Invalid column name"** khi gọi API vì tên cột trong database không khớp với entity models trong C#.

Lỗi mẫu:
```
Invalid column name 'ExecutedAt'.
Invalid column name 'InputJson'.
Invalid column name 'OutputJson'.
Invalid column name 'PackageID'.
Invalid column name 'ProcessingTimeMs'.
Invalid column name 'PromptName'.
Invalid column name 'Status'.
Invalid column name 'UserID'.
```

## Giải Pháp

Có 3 cách để fix:

### ✅ Cách 1: Chạy Script SQL (KHUYÊN DÙNG)

Script này sẽ tự động rename và xoá các cột không cần thiết.

#### Bước 1: Mở SQL Server Management Studio (SSMS)

#### Bước 2: Chạy script kiểm tra schema hiện tại
```sql
-- File: Note/check_current_schema.sql
```
Mở file này trong SSMS và execute (F5) để xem structure hiện tại.

#### Bước 3: Chạy script đồng bộ hoá
```sql
-- File: Note/full_database_sync.sql
```
Mở file này trong SSMS và execute (F5). Script sẽ:
- ✓ Rename các cột để match với entity models
- ✓ Xoá các cột không cần thiết (InputJson, OutputJson, PackageID, PromptName)
- ✓ Thêm PostType và Tags vào Posts table
- ✓ Verify tất cả các bảng cần thiết

#### Bước 4: Restart backend
```powershell
cd D:\eduprompt-be\Eduprompt.API
dotnet watch run
```

#### Bước 5: Test API
Mở Swagger: http://localhost:5217/swagger
Test endpoint: `GET /api/AIHistory`

---

### Cách 2: Sử dụng EF Core Migrations (Nâng Cao)

Nếu bạn muốn dùng EF Core migrations chính thức:

#### Bước 1: Cài dotnet-ef tool
```powershell
dotnet tool install --global dotnet-ef
```

#### Bước 2: Tạo migration
```powershell
cd D:\eduprompt-be\Eduprompt.API
dotnet ef migrations add SyncDatabaseSchema --project ../Eduprompt.DAL/Eduprompt.DAL.csproj --startup-project . --context EdupromptContext
```

#### Bước 3: Áp dụng migration
```powershell
dotnet ef database update --project ../Eduprompt.DAL/Eduprompt.DAL.csproj --startup-project . --context EdupromptContext
```

---

### Cách 3: Thêm Column Attributes (Tạm Thời)

Nếu bạn không thể thay đổi database, có thể thêm `[Column]` attributes vào entity:

```csharp
// Eduprompt.Domain/Entities/AIHistory.cs

[Table("AIHistories")]
public partial class AIHistory
{
    [Key]
    [Column("AIHistoryID")]
    public int AIHistoryID { get; set; }

    [Required]
    [Column("UserId")] // Map to existing column name in DB
    public int UserID { get; set; }

    [Column("CreatedDate")] // Map to existing column name in DB
    public DateTime ExecutedAt { get; set; }

    // ... etc
}
```

**Lưu ý:** Cách này chỉ nên dùng tạm thời vì sẽ gây confusion giữa property name và column name.

---

## Các Script SQL Có Sẵn

1. **check_current_schema.sql** - Xem structure database hiện tại
2. **full_database_sync.sql** - Đồng bộ toàn bộ schema (KHUYÊN DÙNG)
3. **rename_aihistory_columns.sql** - Chỉ rename AIHistories table
4. **seed_dev.sql** - Nạp sample data (chạy sau khi sync schema)

---

## Cấu Trúc AIHistories Đúng

Sau khi sync, bảng AIHistories phải có các cột:

| Column Name       | Data Type      | Nullable | Default      |
|-------------------|----------------|----------|--------------|
| AIHistoryID       | INT            | NO       | IDENTITY     |
| UserID            | INT            | NO       | -            |
| ConversationID    | INT            | YES      | NULL         |
| PromptInstanceID  | INT            | YES      | NULL         |
| UserMessage       | NVARCHAR(MAX)  | YES      | NULL         |
| AIResponse        | NVARCHAR(MAX)  | YES      | NULL         |
| ExecutedAt        | DATETIME2      | NO       | GETUTCDATE() |
| ProcessingTimeMs  | INT            | YES      | NULL         |
| Status            | NVARCHAR(50)   | YES      | 'Completed'  |

---

## Troubleshooting

### Lỗi: "Foreign key constraint"
Nếu không rename được vì foreign key:
1. Drop foreign key trước
2. Rename column
3. Tạo lại foreign key

Script đã handle trường hợp này.

### Lỗi: "Cannot drop column because it is referenced"
Backup data, drop column, recreate với tên mới, restore data.

### Lỗi: "AIHistories table does not exist"
Chạy `seed_dev.sql` hoặc tạo database từ đầu với EF Core.

---

## Checklist

- [ ] Đã chạy `check_current_schema.sql` để xem structure hiện tại
- [ ] Đã chạy `full_database_sync.sql` để đồng bộ schema
- [ ] Đã verify structure trong output của script
- [ ] Đã restart backend: `dotnet watch run`
- [ ] Đã test API trong Swagger
- [ ] API `/api/AIHistory` trả về 200 OK (hoặc empty array nếu chưa có data)
- [ ] Nếu cần data, chạy `seed_dev.sql`

---

## Khi Nào Cần Sync Lại?

- Khi thêm/sửa/xoá properties trong Entity classes
- Khi thay đổi tên property
- Khi gặp lỗi "Invalid column name"
- Sau khi pull code mới từ git có thay đổi entities

---

## Liên Hệ

Nếu vẫn gặp lỗi, cung cấp:
1. Output của `check_current_schema.sql`
2. Lỗi chi tiết từ backend console
3. Endpoint đang gọi

