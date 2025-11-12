# Admin Dashboard - Endpoints Analysis

## Tổng quan
Phân tích tất cả endpoints có thể tích hợp vào admin dashboard để quản lý hệ thống.

---

## 1. USERS MANAGEMENT
**Endpoints:**
- `GET /api/users` - Lấy tất cả users (Admin only)
- `GET /api/users/{id}` - Lấy user theo ID
- `POST /api/users` - Tạo user mới (Admin only)
- `PUT /api/users/{id}` - Cập nhật user (Admin only)
- `DELETE /api/users/{id}` - Xóa user (Admin only)

**Chức năng quản lý:**
- ✅ Xem danh sách users
- ✅ Tìm kiếm/filter users
- ✅ Xem chi tiết user
- ✅ Tạo user mới
- ✅ Cập nhật thông tin user
- ✅ Xóa user
- ✅ Xem role của user

---

## 2. ORDERS MANAGEMENT
**Endpoints:**
- `GET /api/orders` - Lấy tất cả orders (Admin only)
- `GET /api/orders/{orderId}` - Lấy order theo ID
- `GET /api/orders/admin/{orderId}` - Lấy order (Admin only)
- `PATCH /api/orders/{orderId}/status` - Cập nhật status (Admin only)

**Chức năng quản lý:**
- ✅ Xem danh sách orders
- ✅ Filter theo status (Pending, Paid, Completed, Cancelled)
- ✅ Xem chi tiết order
- ✅ Cập nhật order status
- ✅ Xem items trong order
- ✅ Xem user của order

---

## 3. TRANSACTIONS MANAGEMENT
**Endpoints:**
- `GET /api/transactions` - Lấy tất cả transactions (Admin only)
- `GET /api/transactions/{id}` - Lấy transaction theo ID
- `GET /api/transactions/user/{UserId}` - Lấy transactions của user
- `GET /api/transactions/wallet/{WalletId}` - Lấy transactions của wallet

**Chức năng quản lý:**
- ✅ Xem danh sách transactions
- ✅ Filter theo type (TopUp, Payment, Deposit, ExternalPayment)
- ✅ Filter theo status (Pending, Completed, Failed)
- ✅ Xem chi tiết transaction
- ✅ Xem transactions của user/wallet

---

## 4. PAYMENTS MANAGEMENT
**Endpoints:**
- `GET /api/payments` - Lấy tất cả payments (Admin only)
- `GET /api/payments/{paymentId}` - Lấy payment theo ID
- `GET /api/payments/orders/{orderId}` - Lấy payments của order
- `PATCH /api/payments/{paymentId}/status` - Cập nhật status (Admin only)

**Chức năng quản lý:**
- ✅ Xem danh sách payments
- ✅ Filter theo status (Pending, Paid, Failed)
- ✅ Filter theo provider (VNPay, etc.)
- ✅ Xem chi tiết payment
- ✅ Cập nhật payment status

---

## 5. WALLETS MANAGEMENT
**Endpoints:**
- `GET /api/wallets/user/{UserId}` - Lấy wallet của user (Admin or own)
- `GET /api/wallets/{WalletId}` - Lấy wallet theo ID
- `GET /api/wallets/balance/{UserId}` - Lấy balance (Admin or own)
- `POST /api/wallets/add-funds` - Nạp tiền (Admin or own)
- `POST /api/wallets/deduct-funds` - Trừ tiền (Admin or own)
- `PUT /api/wallets/{WalletId}` - Cập nhật wallet
- `DELETE /api/wallets/{WalletId}` - Xóa wallet

**Chức năng quản lý:**
- ✅ Xem danh sách wallets của tất cả users
- ✅ Xem balance của từng user
- ✅ Nạp/trừ tiền cho user
- ✅ Xem lịch sử transactions của wallet
- ✅ Cập nhật wallet status

---

## 6. PACKAGES MANAGEMENT
**Endpoints:**
- `GET /api/packages` - Lấy tất cả packages
- `GET /api/packages/{PackageId}` - Lấy package theo ID
- `POST /api/packages` - Tạo package mới (Admin only)
- `PUT /api/packages/{PackageId}` - Cập nhật package (Admin only)
- `DELETE /api/packages/{PackageId}` - Xóa package (Admin only)

**Chức năng quản lý:**
- ✅ Xem danh sách packages
- ✅ Tìm kiếm/filter packages
- ✅ Xem chi tiết package
- ✅ Tạo package mới
- ✅ Cập nhật package
- ✅ Xóa package
- ✅ Xem package details

---

## 7. PACKAGE CATEGORIES MANAGEMENT
**Endpoints:**
- `GET /api/package-categories` - Lấy tất cả categories
- `GET /api/package-categories/{id}` - Lấy category theo ID
- `POST /api/package-categories` - Tạo category mới
- `PUT /api/package-categories/{id}` - Cập nhật category
- `DELETE /api/package-categories/{id}` - Xóa category

**Chức năng quản lý:**
- ✅ Xem danh sách categories
- ✅ Tạo category mới
- ✅ Cập nhật category
- ✅ Xóa category
- ✅ Xem số packages trong category

---

## 8. POSTS MANAGEMENT
**Endpoints:**
- `GET /api/posts` - Lấy tất cả posts
- `GET /api/posts/{PostId}` - Lấy post theo ID
- `GET /api/posts/published` - Lấy published posts
- `GET /api/posts/search` - Tìm kiếm posts
- `DELETE /api/posts/{PostId}` - Xóa post

**Chức năng quản lý:**
- ✅ Xem danh sách posts
- ✅ Tìm kiếm posts
- ✅ Filter theo status (Published, Draft, etc.)
- ✅ Xem chi tiết post
- ✅ Xóa post
- ✅ Xem reviews của post

---

## 9. STORAGE TEMPLATES MANAGEMENT
**Endpoints:**
- `GET /api/storage-templates/public` - Lấy public templates
- `GET /api/storage-templates/my-storage` - Lấy templates của user
- `DELETE /api/storage-templates/{id}` - Xóa template
- `PATCH /api/storage-templates/{id}` - Cập nhật template
- `POST /api/storage-templates/{id}/publish` - Publish template
- `POST /api/storage-templates/{id}/unpublish` - Unpublish template

**Chức năng quản lý:**
- ✅ Xem danh sách templates (public + private)
- ✅ Filter theo public/private
- ✅ Xem chi tiết template
- ✅ Xóa template
- ✅ Publish/Unpublish template
- ✅ Cập nhật template

---

## 10. PROMPT INSTANCES MANAGEMENT
**Endpoints:**
- `GET /api/prompt-instances` - Lấy tất cả instances
- `GET /api/prompt-instances/{InstanceId}` - Lấy instance theo ID
- `GET /api/prompt-instances/user/{UserId}` - Lấy instances của user
- `GET /api/prompt-instances/status/{status}` - Lấy instances theo status
- `DELETE /api/prompt-instances/{InstanceId}` - Xóa instance

**Chức năng quản lý:**
- ✅ Xem danh sách prompt instances
- ✅ Filter theo status (Pending, Completed, etc.)
- ✅ Filter theo user
- ✅ Xem chi tiết instance
- ✅ Xóa instance

---

## 11. AI HISTORIES MANAGEMENT
**Endpoints:**
- `GET /api/ai-histories` - Lấy tất cả histories (Admin only)
- `GET /api/ai-histories/{id}` - Lấy history theo ID
- `GET /api/ai-histories/user/{UserId}` - Lấy histories của user
- `GET /api/ai-histories/instance/{InstanceId}` - Lấy histories của instance
- `DELETE /api/ai-histories/{id}` - Xóa history

**Chức năng quản lý:**
- ✅ Xem danh sách AI histories
- ✅ Filter theo user/instance
- ✅ Xem chi tiết history
- ✅ Xóa history
- ✅ Xem stats của user

---

## 12. FEEDBACKS/REVIEWS MANAGEMENT
**Endpoints:**
- `GET /api/feedbacks/post/{PostId}` - Lấy feedbacks của post
- `GET /api/feedbacks/storage/{StorageId}` - Lấy feedbacks của storage
- `GET /api/feedbacks/user/{UserId}` - Lấy feedbacks của user
- `GET /api/feedbacks/{id}` - Lấy feedback theo ID
- `DELETE /api/feedbacks/{id}` - Xóa feedback

**Chức năng quản lý:**
- ✅ Xem danh sách feedbacks/reviews
- ✅ Filter theo post/storage/user
- ✅ Xem rating trung bình
- ✅ Xem chi tiết feedback
- ✅ Xóa feedback

---

## 13. API KEYS MANAGEMENT
**Endpoints:**
- `GET /api/api-keys/package/{PackageId}` - Lấy API keys của package
- `GET /api/api-keys/active/package/{PackageId}` - Lấy active API keys
- `GET /api/api-keys/provider/{provider}` - Lấy API keys theo provider
- `POST /api/api-keys` - Tạo API key mới
- `PUT /api/api-keys/{apiKeyId}` - Cập nhật API key
- `DELETE /api/api-keys/{apiKeyId}` - Xóa API key

**Chức năng quản lý:**
- ✅ Xem danh sách API keys
- ✅ Filter theo package/provider
- ✅ Tạo API key mới
- ✅ Cập nhật API key
- ✅ Xóa API key

---

## 14. ROLES MANAGEMENT
**Endpoints:**
- `GET /api/roles` - Lấy tất cả roles
- `GET /api/roles/{id}` - Lấy role theo ID
- `POST /api/roles` - Tạo role mới
- `PUT /api/roles/{id}` - Cập nhật role
- `DELETE /api/roles/{id}` - Xóa role

**Chức năng quản lý:**
- ✅ Xem danh sách roles
- ✅ Tạo role mới
- ✅ Cập nhật role
- ✅ Xóa role

---

## 15. PAYMENT METHODS MANAGEMENT
**Endpoints:**
- `GET /api/payment-methods` - Lấy tất cả payment methods (Admin only)
- `GET /api/payment-methods/{id}` - Lấy payment method theo ID
- `POST /api/payment-methods` - Tạo payment method mới
- `PUT /api/payment-methods/{id}` - Cập nhật payment method
- `DELETE /api/payment-methods/{id}` - Xóa payment method

**Chức năng quản lý:**
- ✅ Xem danh sách payment methods
- ✅ Tạo payment method mới
- ✅ Cập nhật payment method
- ✅ Xóa payment method

---

## 16. TEMPLATE ARCHITECTURES MANAGEMENT
**Endpoints:**
- `GET /api/template-architectures` - Lấy tất cả architectures (Admin only)
- `GET /api/template-architectures/{id}` - Lấy architecture theo ID
- `POST /api/template-architectures` - Tạo architecture mới (Admin only)
- `PUT /api/template-architectures/{architectureId}` - Cập nhật (Admin only)
- `DELETE /api/template-architectures/{architectureId}` - Xóa (Admin only)

**Chức năng quản lý:**
- ✅ Xem danh sách template architectures
- ✅ Tạo architecture mới
- ✅ Cập nhật architecture
- ✅ Xóa architecture

---

## 17. DASHBOARD OVERVIEW
**Stats cần hiển thị:**
- Tổng số users
- Tổng số packages
- Tổng số orders
- Tổng số transactions
- Tổng số templates
- Tổng số AI histories
- Tổng doanh thu (từ orders/payments)
- Tổng số wallets
- Số orders pending/paid/completed
- Số transactions theo type

---

## CẤU TRÚC SIDEBAR NAVIGATION ĐỀ XUẤT

```
📊 Dashboard
├── Overview (Stats & Charts)

👥 Users
├── All Users
├── Create User
└── User Roles

📦 Content
├── Packages
│   ├── All Packages
│   ├── Create Package
│   └── Package Categories
├── Templates
│   ├── Storage Templates
│   ├── Prompt Instances
│   └── Template Architectures
└── Posts
    ├── All Posts
    └── Published Posts

💰 Orders & Payments
├── Orders
│   ├── All Orders
│   └── Order Details
├── Transactions
│   ├── All Transactions
│   └── By Type/Status
├── Payments
│   ├── All Payments
│   └── By Status
└── Wallets
    ├── All Wallets
    └── Wallet Management

🤖 AI & Analytics
├── AI Histories
├── AI Stats
└── User Analytics

💬 Reviews & Feedback
├── All Reviews
├── Post Reviews
└── Template Reviews

⚙️ System Settings
├── API Keys
├── Payment Methods
└── Roles
```

---

## UI/UX IMPROVEMENTS

1. **Sidebar Navigation:**
   - Collapsible categories
   - Icons cho mỗi menu item
   - Active state rõ ràng
   - Search trong sidebar

2. **Dashboard Overview:**
   - Stats cards với icons
   - Charts (line, bar, pie)
   - Recent activities
   - Quick actions

3. **Data Tables:**
   - Search bar
   - Filter dropdowns
   - Pagination
   - Sort columns
   - Bulk actions
   - Export data

4. **Forms:**
   - Validation
   - Loading states
   - Success/Error messages
   - Auto-save drafts

5. **Details View:**
   - Modal hoặc separate page
   - Tabs cho different sections
   - Related data
   - Action buttons

6. **Colors & Theme:**
   - Consistent color scheme
   - Dark mode support
   - Hover effects
   - Transitions/Animations

---

## NEXT STEPS

1. ✅ Phân tích endpoints - DONE
2. ⏳ Thiết kế lại cấu trúc sidebar
3. ⏳ Tạo Dashboard Overview component
4. ⏳ Tạo các Management pages
5. ⏳ Cải thiện UI/UX
6. ⏳ Thêm search, filter, pagination
7. ⏳ Testing

