# ⚡ Quick Start - EdupromptV2 Database

## 🎯 Giải Pháp Cho Lỗi "Invalid Column Name"

Thay vì sửa database cũ, tạo **database mới hoàn toàn từ entities** (Code First).

---

## 3 Bước Nhanh Nhất

### 1️⃣ Tạo Database
```sql
-- Chạy trong SSMS
D:/eduprompt-be/Note/CREATE_EDUPROMPT_V2_DATABASE.sql
```
**Kết quả:** Database `EdupromptV2` với 22 tables

---

### 2️⃣ Nạp Data Mẫu
```sql
-- Chạy trong SSMS
D:/eduprompt-be/Note/SEED_EDUPROMPT_V2_DATA.sql
```
**Kết quả:** 5 users, 5 packages, orders, transactions, AI histories...

---

### 3️⃣ Đổi Connection String

File: `appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=EdupromptV2;..."
}
```

Chỉ đổi: `Eduprompt` → `EdupromptV2`

---

## 🚀 Restart & Test

```powershell
cd D:\eduprompt-be\Eduprompt.API
dotnet watch run
```

Test: http://localhost:5217/swagger

Login:
- **Email:** `admin@eduprompt.com`
- **Password:** `Password123`

---

## ✅ Done!

- ✅ Không còn lỗi "Invalid column name"
- ✅ Tất cả APIs hoạt động
- ✅ Code First approach
- ✅ Clean database schema

---

**Xem chi tiết:** `SWITCH_TO_V2_GUIDE.md`

