# 📊 TỔNG HỢP TẤT CẢ STATUS TRONG EDUPROMPT API

## 🎯 Tổng quan

Hệ thống Eduprompt sử dụng các Status để quản lý trạng thái của các entities khác nhau. Dưới đây là danh sách đầy đủ tất cả các Status được sử dụng trong project.

---

## 📋 1. USER STATUS (Users Table)

**Trường**: `Status` (NVARCHAR(50))

### Giá trị:
- ✅ **Active** - Tài khoản đang hoạt động
- ❌ **Inactive** - Tài khoản bị vô hiệu hóa
- 🚫 **Banned** - Tài khoản bị cấm
- ⏸️ **Suspended** - Tài khoản tạm ngưng

**Mặc định**: `Active`

---

## 🔐 2. ROLE STATUS (Roles Table)

**Trường**: `Status` (NVARCHAR(50))

### Giá trị:
- ✅ **Active** - Role đang hoạt động
- ❌ **Inactive** - Role bị vô hiệu hóa

**Mặc định**: `Active`

---

## 🛒 3. CART STATUS (Carts Table)

**Trường**: `Status` (NVARCHAR(50))

### Giá trị:
- ✅ **Active** - Giỏ hàng đang hoạt động
- 📦 **Abandoned** - Giỏ hàng bị bỏ rơi
- 🧹 **Cleared** - Giỏ hàng đã được xóa
- ✅ **Checkout** - Đang thanh toán

**Mặc định**: `Active`

---

## 💬 4. CONVERSATION STATUS (Conversations Table)

**Trường**: `Status` (NVARCHAR(50))

### Giá trị:
- ✅ **Active** - Cuộc trò chuyện đang diễn ra
- 📂 **Archived** - Đã lưu trữ
- ❌ **Closed** - Đã đóng

**Mặc định**: `Active`

---

## 📨 5. MESSAGE STATUS (Messages Table)

**Trường**: `Status` (NVARCHAR(50))

### Giá trị:
- ✅ **Sent** - Đã gửi
- 📬 **Delivered** - Đã gửi thành công
- ✅ **Read** - Đã đọc
- ❌ **Failed** - Gửi thất bại
- 🗑️ **Deleted** - Đã xóa

**Mặc định**: `Sent`

---

## 📦 6. ORDER STATUS (Orders Table)

**Trường**: `Status` (NVARCHAR(50))

### Giá trị:
- ⏳ **Pending** - Chờ xử lý
- 💳 **Processing** - Đang xử lý thanh toán
- ✅ **Completed** - Đã hoàn thành
- ❌ **Cancelled** - Đã hủy
- 🔄 **Refunded** - Đã hoàn tiền
- ❌ **Failed** - Thanh toán thất bại

**Mặc định**: `Pending`

---

## 💰 7. TRANSACTION STATUS (Transactions Table)

**Trường**: `Status` (NVARCHAR(50))

### Giá trị:
- ⏳ **Pending** - Đang chờ xử lý
- ✅ **Completed** - Đã hoàn thành
- ❌ **Failed** - Thất bại
- 🔄 **Reversed** - Đã đảo ngược
- ⏸️ **OnHold** - Tạm giữ

**Mặc định**: `Pending`

---

## 💸 8. WALLET STATUS (Wallets Table)

**Trường**: `Status` (NVARCHAR(50))

### Giá trị:
- ✅ **Active** - Ví đang hoạt động
- 🔒 **Frozen** - Ví bị đóng băng
- ❌ **Closed** - Ví đã đóng
- ⚠️ **Suspended** - Ví tạm ngưng

**Mặc định**: `Active`

---

## 🤖 9. AI HISTORY STATUS (AIHistories Table)

**Trường**: `Status` (NVARCHAR(50))

### Giá trị:
- ✅ **Completed** - Đã hoàn thành
- ⏳ **Processing** - Đang xử lý
- ❌ **Failed** - Thất bại
- ⏸️ **Paused** - Tạm dừng
- ⚠️ **Error** - Lỗi

**Mặc định**: `Completed`

---

## 🎯 10. PROMPT INSTANCE STATUS (PromptInstances Table)

**Trường**: `Status` (NVARCHAR(50))

### Giá trị:
- ✅ **Completed** - Đã hoàn thành
- ⏳ **Running** - Đang chạy
- ⏳ **Queued** - Đang chờ trong hàng đợi
- ❌ **Failed** - Thất bại
- ⏸️ **Paused** - Tạm dừng
- 🔄 **Retrying** - Đang thử lại

**Mặc định**: `Completed`

---

## 📝 11. POST STATUS (Posts Table)

**Trường**: `Status` (NVARCHAR(50))

### Giá trị:
- ✅ **Published** - Đã xuất bản
- 📝 **Draft** - Bản nháp
- 🔄 **Reviewing** - Đang duyệt
- ❌ **Rejected** - Bị từ chối
- 🔒 **Archived** - Đã lưu trữ
- ⏸️ **Unpublished** - Không xuất bản

**Mặc định**: `Published`

---

## 👍 12. FEEDBACK STATUS (Feedbacks Table)

**Trường**: `Status` (NVARCHAR(50))

### Giá trị:
- ✅ **Active** - Phản hồi đang hoạt động
- 🚫 **Removed** - Đã bị xóa
- ⚠️ **Flagged** - Đã được gắn cờ
- 🔒 **Hidden** - Đã ẩn

**Mặc định**: `Active`

---

## 🎨 13. PACKAGE STATUS (Implicit - IsActive flag)

**Trường**: `IsActive` (BIT) - không phải Status nhưng tương tự

### Giá trị:
- ✅ **1 (true)** - Package đang hoạt động, có sẵn
- ❌ **0 (false)** - Package bị vô hiệu hóa, không hiển thị

**Mặc định**: `1 (Active)`

---

## 📊 TỔNG KẾT CÁC STATUS

### Status Đang hoạt động (Active):
- `Active` - Dùng cho: Users, Roles, Carts, Conversations, Wallets, Feedbacks
- `Sent` - Dùng cho: Messages
- `Published` - Dùng cho: Posts
- `Completed` - Dùng cho: AIHistories, PromptInstances, Transactions, Orders

### Status Chờ xử lý (Pending):
- `Pending` - Dùng cho: Orders, Transactions
- `Draft` - Dùng cho: Posts
- `Queued` - Dùng cho: PromptInstances
- `Processing` - Dùng cho: Orders, Transactions, AIHistories
- `Running` - Dùng cho: PromptInstances

### Status Thất bại (Failed):
- `Failed` - Dùng cho: Messages, Orders, Transactions, AIHistories, PromptInstances
- `Error` - Dùng cho: AIHistories
- `Rejected` - Dùng cho: Posts

### Status Đã đóng/Bị chặn:
- `Inactive` - Dùng cho: Users, Roles
- `Cancelled` - Dùng cho: Orders
- `Closed` - Dùng cho: Conversations, Wallets
- `Archived` - Dùng cho: Conversations, Posts
- `Frozen` / `Suspended` - Dùng cho: Wallets
- `Banned` - Dùng cho: Users
- `Removed` / `Flagged` / `Hidden` - Dùng cho: Feedbacks

### Status Đặc biệt:
- `Refunded` - Đã hoàn tiền (Orders)
- `Reversed` - Đã đảo ngược (Transactions)
- `Retrying` - Đang thử lại (PromptInstances)
- `Reviewing` - Đang duyệt (Posts)
- `Delivered` - Đã gửi thành công (Messages)

---

## 🎯 Status Flow Patterns

### 1. Order Flow:
```
Pending → Processing → Completed
                     ↓
                   Cancelled / Failed / Refunded
```

### 2. Transaction Flow:
```
Pending → Completed / Failed
                     ↓
                   Reversed
```

### 3. Prompt Instance Flow:
```
Queued → Running → Completed
                  ↓
               Failed → Retrying → Completed
```

### 4. Post Flow:
```
Draft → Reviewing → Published
                  ↓
               Rejected / Unpublished
```

### 5. User Flow:
```
Active → Suspended / Banned / Inactive
```

---

## 📌 Ghi chú quan trọng:

1. **Mặc định**: Hầu hết Status mặc định là `Active` hoặc `Pending` hoặc `Completed`

2. **Case Sensitive**: Status KHÔNG phân biệt chữ hoa/thường trong database

3. **Format**: Tất cả Status đều là NVARCHAR(50) để linh hoạt

4. **Migration**: Status được quản lý bởi string, không phải enum, để dễ thêm status mới

5. **Validation**: Frontend nên validate status trước khi gửi request

---

## 🔍 Cách sử dụng Status:

### GET entities by status:
```sql
-- Get active users
SELECT * FROM Users WHERE Status = 'Active'

-- Get pending orders
SELECT * FROM Orders WHERE Status = 'Pending'

-- Get completed transactions
SELECT * FROM Transactions WHERE Status = 'Completed'
```

### Update status:
```csharp
order.Status = "Completed";
orderService.Update(order);
```

### Check status:
```csharp
if (order.Status == "Pending")
{
    // Process order
}
```

---

## 🎨 UI Status Colors (Suggested):

- **Active/Published/Completed**: 🟢 Green
- **Pending/Processing**: 🟡 Yellow
- **Failed/Error/Cancelled**: 🔴 Red
- **Inactive/Archived**: ⚫ Gray
- **Suspended**: 🟠 Orange

---

**File được tạo**: 2025-01-17
**Project**: Eduprompt Backend API
**Version**: 1.0

