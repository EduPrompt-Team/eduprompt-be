# Hướng Dẫn Chuyển Sang EdupromptV2 Database

## 🎯 Tại Sao Cần EdupromptV2?

Database cũ có các vấn đề:
- ❌ Tên cột không khớp với entities (UserId vs UserID, CreatedDate vs ExecutedAt, etc.)
- ❌ Có thêm các cột không cần thiết (InputJson, OutputJson trong AIHistories)
- ❌ Khó maintain và debug

Database mới (V2):
- ✅ **Code First Approach** - được tạo 100% từ C# entities
- ✅ Tên cột khớp hoàn toàn với code
- ✅ Clean schema, không có cột thừa
- ✅ Dễ mở rộng và maintain

---

## 📋 Các Bước Thực Hiện

### Bước 1: Chạy Script Tạo Database

1. Mở **SQL Server Management Studio (SSMS)**
2. Mở file: `D:/eduprompt-be/Note/CREATE_EDUPROMPT_V2_DATABASE.sql`
3. Nhấn **F5** để execute
4. Chờ script chạy xong (khoảng 5-10 giây)
5. Verify: Sẽ thấy database `EdupromptV2` với 22 tables

---

### Bước 2: Seed Sample Data (Tuỳ Chọn)

1. Mở file: `D:/eduprompt-be/Note/SEED_EDUPROMPT_V2_DATA.sql`
2. Nhấn **F5** để execute
3. Script sẽ nạp:
   - 5 users (password: `Password123`)
   - 5 packages
   - Orders, transactions, conversations, AI histories...
   - Tất cả data cần để test APIs

---

### Bước 3: Cập Nhật Connection String

Mở file: `D:/eduprompt-be/Eduprompt.API/appsettings.json`

**TRƯỚC:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=Eduprompt;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

**SAU:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=EdupromptV2;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

**Chỉ cần đổi:** `Database=Eduprompt` → `Database=EdupromptV2`

---

### Bước 4: Restart Backend

```powershell
# Stop backend hiện tại (Ctrl+C nếu đang chạy)

# Start lại
cd D:\eduprompt-be\Eduprompt.API
dotnet watch run
```

Hoặc nếu đang dùng Visual Studio: **Stop** và **Start** lại project.

---

### Bước 5: Test APIs

1. Mở Swagger: http://localhost:5217/swagger

2. **Test AIHistory API** (cái đang lỗi trước đây):
   ```
   GET /api/AIHistory
   ```
   Kết quả: ✅ **200 OK** với data hoặc empty array `[]`

3. **Test Login**:
   ```
   POST /api/auth/login
   {
     "email": "admin@eduprompt.com",
     "password": "Password123"
   }
   ```
   Kết quả: ✅ **200 OK** với accessToken và refreshToken

4. **Test Other APIs**:
   - `GET /api/Package` - Xem danh sách packages
   - `GET /api/Transaction` - Xem transactions
   - `GET /api/Post` - Xem posts
   - `GET /api/Wishlists/check/1` - Check wishlist

---

## 🎨 Test Accounts

Sau khi seed data, bạn có thể login với:

| Email | Password | Role |
|-------|----------|------|
| admin@eduprompt.com | Password123 | Admin |
| nguyenvana@example.com | Password123 | User |
| tranthib@example.com | Password123 | Premium |
| levanc@example.com | Password123 | User |
| phamthid@example.com | Password123 | User |

---

## 🔍 So Sánh Database Cũ vs Mới

### AIHistories Table

**Database Cũ (Eduprompt):**
```
UserId (camelCase)
CreatedDate
TokensUsed
ResponseStatus
InputJson (không cần)
OutputJson (không cần)
PackageID (không cần)
PromptName (không cần)
```

**Database Mới (EdupromptV2):**
```
UserID (PascalCase - khớp với entity)
ExecutedAt (khớp với entity property)
ProcessingTimeMs (khớp với entity property)
Status (khớp với entity property)
✅ Không có cột thừa
✅ Tên cột 100% khớp với C# code
```

---

## ⚠️ Lưu Ý

### 1. Không Cần Xoá Database Cũ

Database cũ `Eduprompt` vẫn giữ nguyên. Bạn có thể quay lại bất cứ lúc nào bằng cách đổi connection string.

### 2. Migration Dữ Liệu Từ DB Cũ (Nếu Cần)

Nếu muốn copy data từ database cũ sang mới:

```sql
-- Example: Copy Users
USE EdupromptV2;

INSERT INTO Users (RoleId, FullName, Email, Phone, ProfileUrl, CreatedDate, Status, Password)
SELECT RoleId, FullName, Email, Phone, ProfileUrl, CreatedDate, Status, Password
FROM Eduprompt.dbo.Users;

-- Tương tự cho các bảng khác...
```

### 3. Entity Properties Phải Khớp

Nếu thêm property mới vào entity:

1. Thêm property trong C# entity
2. Chạy script ALTER TABLE trong SQL để thêm cột
3. Hoặc drop database và chạy lại CREATE script

---

## 🐛 Troubleshooting

### Lỗi: "Cannot open database 'EdupromptV2'"

**Nguyên nhân:** Chưa chạy CREATE script hoặc database name sai.

**Fix:**
1. Kiểm tra database name trong SSMS
2. Chạy lại `CREATE_EDUPROMPT_V2_DATABASE.sql`

### Lỗi: "Login failed for user"

**Nguyên nhân:** SQL Server authentication issue.

**Fix:**
Kiểm tra connection string:
- Dùng Windows Auth: `Trusted_Connection=True`
- Hoặc SQL Auth: `User Id=sa;Password=yourpassword`

### API vẫn trả về lỗi column name

**Nguyên nhân:** Backend chưa restart hoặc vẫn đọc từ DB cũ.

**Fix:**
1. Verify connection string đã đổi sang `EdupromptV2`
2. Stop và start lại backend (không phải chỉ rebuild)
3. Check log console xem đang connect vào DB nào

### Empty data sau khi chạy

**Nguyên nhân:** Chưa chạy seed script.

**Fix:**
Chạy `SEED_EDUPROMPT_V2_DATA.sql`

---

## ✅ Checklist

Hoàn thành các bước sau theo thứ tự:

- [ ] Chạy `CREATE_EDUPROMPT_V2_DATABASE.sql` trong SSMS
- [ ] Verify database `EdupromptV2` đã được tạo với 22 tables
- [ ] Chạy `SEED_EDUPROMPT_V2_DATA.sql` để nạp data mẫu (tuỳ chọn)
- [ ] Đổi connection string trong `appsettings.json`
- [ ] Restart backend: `dotnet watch run`
- [ ] Test API `/api/AIHistory` → phải trả về 200 OK
- [ ] Test login với `admin@eduprompt.com / Password123`
- [ ] Test các APIs khác trong Swagger

---

## 🎉 Kết Quả

Sau khi hoàn thành:

✅ **Không còn lỗi "Invalid column name"**  
✅ **Tất cả APIs hoạt động ổn định**  
✅ **Database schema clean và dễ maintain**  
✅ **Code First approach - mọi thứ từ entities**  
✅ **Có data mẫu để test ngay**  

---

## 📞 Cần Hỗ Trợ?

Nếu gặp bất cứ lỗi nào:

1. Copy full error message
2. Chụp ảnh log console
3. Cho biết bước nào đang thực hiện
4. Gửi lại để được hỗ trợ

---

**Happy Coding! 🚀**

