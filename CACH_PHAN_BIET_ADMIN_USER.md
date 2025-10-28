# 🔐 CÁCH PHÂN BIỆT ADMIN VÀ USER TRONG EDUPROMPT

## 📋 TỔNG QUAN

Project Eduprompt sử dụng **Role-Based Access Control (RBAC)** để phân quyền Admin và User.

---

## 🎯 CƠ CHẾ HOẠT ĐỘNG

### 1. Database Structure

#### **Roles Table**
```sql
Roles
├── RoleId (1, 2, 3)
├── RoleName ('Admin', 'User', 'Premium')
└── Status ('Active')
```

**Mặc định có 3 roles:**
- **RoleId = 1** → `Admin` 
- **RoleId = 2** → `User` (Normal user)
- **RoleId = 3** → `Premium` (Premium user)

#### **Users Table**
```sql
Users
├── UserId
├── RoleId → Foreign key đến Roles
├── FullName
├── Email
└── Status
```

**Logic:**
- Mỗi User có 1 RoleId → xác định quyền của user đó
- RoleId = 1 → Admin (có tất cả quyền)
- RoleId = 2 → User (quyền hạn chế)
- RoleId = 3 → Premium (quyền mở rộng)

---

## 🔍 CÁCH KIỂM TRA QUYỀN

### 1. **Qua Database**

```sql
-- Xem tất cả users và role của họ
SELECT 
    u.UserId,
    u.FullName,
    u.Email,
    r.RoleName,
    u.Status
FROM Users u
LEFT JOIN Roles r ON u.RoleId = r.RoleId;

-- Kết quả ví dụ:
-- UserId | FullName       | Email                 | RoleName | Status
-- 1      | Admin User     | a@example.com         | Admin    | Active
-- 2      | Nguyễn Văn A    | nguyenvana@example.com| User     | Active
-- 3      | Trần Thị B      | tranthib@example.com  | Premium  | Active
```

### 2. **Qua JWT Token**

Khi user login, hệ thống tạo JWT token chứa thông tin role:

```csharp
// Trong AuthService.cs - line 132-135
if (user.Role != null)
{
    claims.Add(new Claim(ClaimTypes.Role, user.Role.RoleName));
}
```

**Token chứa:**
- `userId`
- `email`
- `fullName`
- **`role`** ← Đây là phần quan trọng!

**Decode JWT token để xem role:**
```javascript
// Decode tại: https://jwt.io/
// Token chứa role claim như "Admin" hoặc "User"
```

---

## 🛡️ CÁCH HỆ THỐNG PHÂN QUYỀN

### 1. **Authorization Attributes**

Các Controller dùng attributes để phân quyền:

#### **Admin Only:**
```csharp
[Authorize(Roles = "Admin")]  // Chỉ Admin mới truy cập được
public class CategoriesController : ControllerBase
{
    // ...
}
```

#### **User Authenticated:**
```csharp
[Authorize]  // Bất kỳ user nào đã login đều truy cập được
public class CartController : ControllerBase
{
    // ...
}
```

#### **Public (Không cần auth):**
```csharp
[AllowAnonymous]  // Không cần login
public class PackageController : ControllerBase
{
    // ...
}
```

### 2. **Danh sách Endpoints theo Role:**

#### **🔴 CHỈ ADMIN CÓ QUYỀN:**

1. **Roles Controller** - `[Authorize]` (implicit Admin)
   - GET/POST/PUT/DELETE `/api/roles/*`

2. **Categories Controller** - `[Authorize(Roles = "Admin")]`
   - GET/POST/PUT/DELETE `/api/categories/*`
   - GET `/api/categories/root`
   - GET `/api/categories/{id}/subcategories`

3. **Users Controller** - Một số endpoints Admin-only:
   - GET `/api/users` - Xem tất cả users
   - POST `/api/users` - Tạo user mới
   - DELETE `/api/users/{id}` - Xóa user

4. **Package Management** - `[Authorize]` với check Admin:
   - POST `/api/package` - Tạo package mới
   - PUT `/api/package/{id}` - Cập nhật package
   - DELETE `/api/package/{id}` - Xóa package

5. **Orders - Admin View:**
   - GET `/api/order` - Xem tất cả orders
   - GET `/api/order/admin/{id}` - Xem chi tiết bất kỳ order

6. **Transactions:**
   - GET `/api/transaction` - Xem tất cả transactions

7. **API Keys:**
   - GET `/api/apikey/package/{id}` - Xem API keys của package
   - POST/PUT/DELETE `/api/apikey/*`

8. **AI History:**
   - GET `/api/aihistory` - Xem tất cả AI history

9. **Payment Methods:**
   - GET `/api/paymentmethod` - Xem tất cả payment methods

#### **🟢 TẤT CẢ USER ĐĂNG NHẬP:**

- Cart management (`/api/cart/*`)
- Order - My orders (`/api/order/my`)
- Wallet management (`/api/wallet/*`)
- Wishlist (`/api/wishlists/*`)
- Storage (`/api/storage-templates/*`)
- Conversations (`/api/conversation/*`)
- Messages (`/api/message/*`)
- Posts - Create/Update/Delete (`/api/post` - POST/PUT/DELETE)
- Prompt Instances (`/api/promptinstance/*`)
- Feedback (`/api/feedback/*`)
- Auth - Me (`/api/auth/me`)

#### **🟡 PUBLIC (KHÔNG CẦN AUTH):**

- Auth - Register/Login (`/api/auth/register`, `/api/auth/login`)
- Packages - View (`/api/package/*` - GET)
- Posts - View (`/api/post/*` - GET)
- API Keys - Active (`/api/apikey/active/*`)
- Payment Methods - View (một số GET)

---

## 🔑 CÁCH LẤY THÔNG TIN ROLE

### 1. **Trong Controller:**

```csharp
[Authorize]
public class MyController : ControllerBase
{
    [HttpGet("check-role")]
    public IActionResult CheckRole()
    {
        // Lấy user hiện tại từ token
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        
        return Ok(new 
        { 
            UserId = userId,
            Email = email,
            Role = role  // ← "Admin" hoặc "User" hoặc "Premium"
        });
    }
}
```

### 2. **Check nếu là Admin:**

```csharp
[HttpGet("admin-only")]
[Authorize(Roles = "Admin")]  // ← Tự động check role
public IActionResult AdminOnly()
{
    return Ok("You are Admin!");
}
```

### 3. **Check Manual:**

```csharp
public bool IsAdmin()
{
    return User.IsInRole("Admin");
}

public bool IsUser()
{
    return User.IsInRole("User");
}

public bool IsPremium()
{
    return User.IsInRole("Premium");
}
```

---

## 📊 VÍ DỤ THỰC TẾ

### **Seed Data:**

```sql
-- Roles
INSERT INTO Roles (RoleId, RoleName, Status) VALUES
(1, 'Admin', 'Active'),
(2, 'User', 'Active'),
(3, 'Premium', 'Active');

-- Users
INSERT INTO Users (UserId, RoleId, FullName, Email, Status) VALUES
(1, 1, 'Admin User', 'admin@example.com', 'Active'),      -- Admin
(2, 2, 'Nguyễn Văn A', 'user@example.com', 'Active'),    -- User
(3, 3, 'Trần Thị B', 'premium@example.com', 'Active');   -- Premium
```

### **Test Flow:**

1. **Login với Admin** (`admin@example.com`):
   - Token chứa: `"role": "Admin"`
   - Có thể truy cập: Tất cả endpoints

2. **Login với User** (`user@example.com`):
   - Token chứa: `"role": "User"`
   - Có thể truy cập: Chỉ endpoints không có `[Authorize(Roles = "Admin")]`

3. **Login với Premium** (`premium@example.com`):
   - Token chứa: `"role": "Premium"`
   - Có thể truy cập: Tương tự User (trừ khi có quyền đặc biệt)

---

## 🔒 PHÂN QUYỀN CHI TIẾT

### **Admin có thể:**
✅ Quản lý Roles  
✅ Quản lý Categories  
✅ Tạo/Update/Delete Packages  
✅ Xem tất cả Users  
✅ Xem tất cả Orders  
✅ Xem tất cả Transactions  
✅ Quản lý API Keys  
✅ Xem tất cả AI History  
✅ Quản lý Payment Methods  

### **User có thể:**
✅ Quản lý Cart của mình  
✅ Xem Orders của mình  
✅ Quản lý Wallet của mình  
✅ Add to Wishlist  
✅ Lưu Storage  
✅ Tạo Posts  
✅ Tạo Feedback  
✅ Chat (Conversations/Messages)  
✅ Sử dụng Prompt Instances  
❌ KHÔNG quản lý Users  
❌ KHÔNG tạo Packages  
❌ KHÔNG xem tất cả Orders  

### **Public (Chưa login):**
✅ Xem Packages  
✅ Xem Posts  
✅ Xem API Keys active  
✅ Đăng ký/Đăng nhập  
❌ KHÔNG truy cập endpoints cần auth  

---

## 🛠️ CÁCH KIỂM TRA NHANH

### **Query Database:**

```sql
-- Xem role của tất cả users
SELECT 
    u.UserId,
    u.Email,
    u.FullName,
    r.RoleName,
    u.Status,
    CASE 
        WHEN r.RoleName = 'Admin' THEN '🔴 FULL ACCESS'
        WHEN r.RoleName = 'User' THEN '🟢 LIMITED ACCESS'
        WHEN r.RoleName = 'Premium' THEN '🟡 EXTENDED ACCESS'
    END as AccessLevel
FROM Users u
LEFT JOIN Roles r ON u.RoleId = r.RoleId;
```

### **Decode JWT:**

1. Copy JWT token sau khi login
2. Vào https://jwt.io/
3. Paste token
4. Xem claims: `"role": "Admin"` hoặc `"role": "User"`

### **Test với Swagger:**

1. Login với tài khoản Admin
2. Copy token
3. Click "Authorize" trên Swagger UI
4. Paste: `Bearer {token}`
5. Try endpoints cần Admin → ✅ Should work
6. Logout → Login với User
7. Try endpoints Admin → ❌ Should return 403 Forbidden

---

## 🎯 KẾT LUẬN

1. **Database**: Users.RoleId → Roles table
2. **JWT Token**: Chứa claim `ClaimTypes.Role` với giá trị RoleName
3. **Authorization**: Attributes `[Authorize(Roles = "Admin")]` kiểm tra role
4. **Có 3 roles**: Admin, User, Premium
5. **Check role**: Query `User.IsInRole("Admin")` hoặc `ClaimTypes.Role`

---

**Mặc định Admin account**: `a@example.com` (password: `123456`)  
**Default User**: RoleId = 2 cho tất cả user mới đăng ký

---

**Updated**: 2025-01-17  
**Version**: 1.0

