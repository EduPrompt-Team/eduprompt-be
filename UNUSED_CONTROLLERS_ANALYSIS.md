# Phân Tích Controllers Không Được Sử Dụng - 3 Flows Chính

## 📋 3 FLOWS CHÍNH

### 1. **Core Flow**
- Registration/Login → Wallet → Package → Template (Có AI/Không AI) → StorageTemplate
- Controllers: `AuthController`, `WalletController`, `PackageController`, `OrderController`, `TemplateArchitectureController`, `PromptInstanceController`, `AIController`, `StorageTemplatesController`, `CartController`, `PaymentsController`, `ExpectedOutputController`

### 2. **Sell Flow**
- Create Post → Purchase Template → Transaction
- Controllers: `PostController`, `PaymentsController` (VNPay), `TransactionController` (internal)

### 3. **Admin Flow**
- Create Template Architecture → Seed Data
- Controllers: `TemplateArchitectureController`, `UsersController` (có thể cần)

---

## ✅ CONTROLLERS ĐƯỢC SỬ DỤNG (GIỮ LẠI)

| Controller | Flow | Mục đích | Status |
|------------|------|----------|--------|
| `AuthController` | Core | Login/Register/Google Login | ✅ **GIỮ** |
| `WalletController` | Core | Wallet management, balance, add/deduct funds | ✅ **GIỮ** |
| `PackageController` | Core | Package listing, search, CRUD | ✅ **GIỮ** |
| `OrderController` | Core | Create order from cart, pay with wallet, cancel | ✅ **GIỮ** |
| `TemplateArchitectureController` | Core + Admin | Template Architecture CRUD, get by ID | ✅ **GIỮ** |
| `PromptInstanceController` | Core | Prompt Instance CRUD, complete instance | ✅ **GIỮ** |
| `AIController` | Core | AI suggestions generation | ✅ **GIỮ** |
| `StorageTemplatesController` | Core | Storage template management, publish/unpublish | ✅ **GIỮ** |
| `PostController` | Sell | Post CRUD, purchase template, like, rating | ✅ **GIỮ** |
| `CartController` | Core | Shopping cart management (add/update/remove items) | ✅ **GIỮ** - Được sử dụng trong FE/MO |
| `PaymentsController` | Core | VNPay payment URL creation, callback, IPN | ✅ **GIỮ** - Được sử dụng trong FE/MO |
| `ExpectedOutputController` | Core | Expected Output (tự động tạo bởi AI) | ✅ **GIỮ** - Được sử dụng trong FE/MO |
| `TransactionController` | Sell | Transaction history (xem lịch sử giao dịch) | ✅ **GIỮ** - Được sử dụng trong FE/MO |

---

## ❌ CONTROLLERS KHÔNG ĐƯỢC SỬ DỤNG (CÓ THỂ XÓA)

### 1. **AIHistoryController** ❌
- **Route**: `/api/AIHistory`
- **Mục đích**: Lịch sử AI calls
- **Lý do không cần**: 
  - Không được sử dụng trong FE/MO
  - AI history được tạo tự động trong `AIController`, không cần endpoint riêng để query
  - Có thể xóa hoặc giữ lại cho admin dashboard (optional)
- **Recommendation**: ⚠️ **XÓA HOẶC GIỮ CHO ADMIN** (nếu cần xem lịch sử AI)

### 2. **APIKeyController** ❌
- **Route**: `/api/api-keys`
- **Mục đích**: Quản lý API keys
- **Lý do không cần**: 
  - Không liên quan đến 3 flows chính
  - Có thể là feature tương lai, nhưng hiện tại không cần
- **Recommendation**: ✅ **XÓA**

### 3. **CategoriesController** ⚠️
- **Route**: `/api/categories`
- **Mục đích**: Category management (Admin only)
- **Lý do không cần**: 
  - Không được sử dụng trong FE/MO
  - Package categories có thể được quản lý qua `PackageCategoryController`
- **Recommendation**: ⚠️ **XÓA** (nếu `PackageCategoryController` đủ)

### 4. **ConversationController** ❌
- **Route**: `/api/conversations`
- **Mục đích**: Chat/Conversation management
- **Lý do không cần**: 
  - Không liên quan đến 3 flows chính
  - Feature chat không nằm trong scope
- **Recommendation**: ✅ **XÓA**

### 5. **FeedbackController** ⚠️
- **Route**: `/api/feedbacks`
- **Mục đích**: Feedback management (có thể cho Post)
- **Lý do không cần**: 
  - Không được sử dụng trong FE/MO
  - Post đã có rating system riêng
  - Feedback có thể là feature tương lai
- **Recommendation**: ⚠️ **XÓA** (nếu không cần feedback system)

### 6. **MessageController** ❌
- **Route**: `/api/messages`
- **Mục đích**: Message management (chat)
- **Lý do không cần**: 
  - Không liên quan đến 3 flows chính
  - Feature chat không nằm trong scope
- **Recommendation**: ✅ **XÓA**

### 7. **PackageCategoryController** ⚠️
- **Route**: `/api/package-categories`
- **Mục đích**: Package Category management
- **Lý do không cần**: 
  - Không được sử dụng trong FE/MO
  - Package có thể không cần categories phức tạp
- **Recommendation**: ⚠️ **XÓA** (nếu Package không cần categories)

### 8. **PackageDetailController** ⚠️
- **Route**: `/api/package-details`
- **Mục đích**: Package Detail management
- **Lý do không cần**: 
  - Không được sử dụng trong FE/MO
  - Package có thể đủ thông tin trong `PackageController`
- **Recommendation**: ⚠️ **XÓA** (nếu Package không cần details riêng)

### 9. **PaymentMethodController** ⚠️
- **Route**: `/api/payment-methods`
- **Mục đích**: Payment Method management
- **Lý do không cần**: 
  - Không được sử dụng trong FE/MO
  - Payment methods có thể hardcode (VNPay, Wallet)
- **Recommendation**: ⚠️ **XÓA** (nếu không cần dynamic payment methods)

### 10. **PromptInstanceDetailController** ⚠️
- **Route**: `/api/prompt-instance-details`
- **Mục đích**: Prompt Instance Detail management
- **Lý do không cần**: 
  - Không được sử dụng trong FE/MO
  - PromptInstance có thể đủ thông tin trong `PromptInstanceController`
- **Recommendation**: ⚠️ **XÓA** (nếu PromptInstance không cần details riêng)

### 11. **RolesController** ⚠️
- **Route**: `/api/roles`
- **Mục đích**: Role management (Admin only)
- **Lý do không cần**: 
  - Không được sử dụng trong FE/MO
  - Roles có thể hardcode (Admin, User)
- **Recommendation**: ⚠️ **XÓA** (nếu không cần dynamic roles)

### 12. **TemplateCommerceController** ❌
- **Route**: `/api/template-commerce`
- **Mục đích**: Template Commerce (có thể duplicate với PostController)
- **Lý do không cần**: 
  - Không được sử dụng trong FE/MO
  - PostController đã handle template selling
- **Recommendation**: ✅ **XÓA**

### 13. **UsersController** ⚠️
- **Route**: `/api/users`
- **Mục đích**: User management (Admin only)
- **Lý do không cần**: 
  - Không được sử dụng trong FE/MO
  - Có thể cần cho Admin dashboard
- **Recommendation**: ⚠️ **GIỮ CHO ADMIN** (nếu cần quản lý users)

### 14. **WishlistsController** ❌
- **Route**: `/api/wishlists`
- **Mục đích**: Wishlist management
- **Lý do không cần**: 
  - Không liên quan đến 3 flows chính
  - Feature wishlist không nằm trong scope
- **Recommendation**: ✅ **XÓA**

---

## 📊 TỔNG KẾT

### ✅ Giữ Lại (13 Controllers)
1. `AuthController` - Login/Register/Google Login
2. `WalletController` - Wallet management
3. `PackageController` - Package listing/search
4. `OrderController` - Order management
5. `TemplateArchitectureController` - Template Architecture CRUD
6. `PromptInstanceController` - Prompt Instance CRUD
7. `AIController` - AI suggestions
8. `StorageTemplatesController` - Storage template management
9. `PostController` - Post CRUD, purchase
10. `CartController` - Shopping cart (✅ Được sử dụng trong FE/MO)
11. `PaymentsController` - VNPay payment (✅ Được sử dụng trong FE/MO)
12. `ExpectedOutputController` - Expected Output (✅ Được sử dụng trong FE/MO)
13. `TransactionController` - Transaction history (✅ Được sử dụng trong FE/MO)

### ❌ Xóa Chắc Chắn (5 Controllers)
1. `APIKeyController` - Không liên quan
2. `ConversationController` - Không liên quan
3. `MessageController` - Không liên quan
4. `TemplateCommerceController` - Duplicate với PostController
5. `WishlistsController` - Không liên quan

### ⚠️ Xóa Nếu Không Cần (9 Controllers)
1. `AIHistoryController` - Có thể giữ cho admin
2. `CategoriesController` - Xóa nếu PackageCategoryController đủ
3. `FeedbackController` - Xóa nếu không cần feedback system
4. `PackageCategoryController` - Xóa nếu Package không cần categories
5. `PackageDetailController` - Xóa nếu Package không cần details riêng
6. `PaymentMethodController` - Xóa nếu payment methods hardcode
7. `PromptInstanceDetailController` - Xóa nếu PromptInstance không cần details
8. `RolesController` - Xóa nếu roles hardcode
9. `UsersController` - Giữ nếu cần admin quản lý users

---

## 🎯 KHUYẾN NGHỊ

### Xóa Ngay (5 Controllers):
```bash
- APIKeyController.cs
- ConversationController.cs
- MessageController.cs
- TemplateCommerceController.cs
- WishlistsController.cs
```

### Xóa Nếu Không Cần (9 Controllers):
- Kiểm tra xem có cần các features này không:
  - AI History tracking (Admin)
  - Package Categories
  - Feedback system
  - Package Details riêng
  - Dynamic Payment Methods
  - Prompt Instance Details riêng
  - Dynamic Roles
  - User management (Admin)

### Giữ Lại (13 Controllers):
- Tất cả controllers liên quan đến 3 flows chính
- `TransactionController` - Được sử dụng trong FE/MO để xem transaction history

---

## 📝 LƯU Ý

1. **TransactionController**: Có thể cần để admin xem transaction history, nhưng không được sử dụng trong FE/MO. Nên giữ lại cho admin dashboard.

2. **UsersController**: Có thể cần để admin quản lý users, nhưng không được sử dụng trong FE/MO. Nên giữ lại cho admin dashboard.

3. **AIHistoryController**: Có thể cần để admin xem lịch sử AI calls, nhưng không được sử dụng trong FE/MO. Nên giữ lại cho admin dashboard.

4. **ExpectedOutputController**: Được tạo tự động bởi AI, không cần endpoint riêng để query, nhưng có thể cần để update/delete. Nên giữ lại.

---

## ✅ KẾT LUẬN

**Tổng số Controllers**: 27
- **Giữ lại**: 13 (được sử dụng trong 3 flows chính)
- **Xóa chắc chắn**: 5
- **Xóa nếu không cần**: 9

**Recommendation**: 
1. **Xóa ngay 5 controllers**: `APIKeyController`, `ConversationController`, `MessageController`, `TemplateCommerceController`, `WishlistsController`
2. **Review và quyết định 9 controllers**: Tùy vào nhu cầu admin dashboard và features tương lai

---

## 📝 DANH SÁCH XÓA CHI TIẾT

### ✅ XÓA NGAY (5 Controllers):
```
1. APIKeyController.cs
2. ConversationController.cs
3. MessageController.cs
4. TemplateCommerceController.cs
5. WishlistsController.cs
```

### ⚠️ XÓA NẾU KHÔNG CẦN (9 Controllers):
```
1. AIHistoryController.cs - Giữ nếu cần admin xem AI history
2. CategoriesController.cs - Xóa nếu PackageCategoryController đủ
3. FeedbackController.cs - Xóa nếu không cần feedback system
4. PackageCategoryController.cs - Xóa nếu Package không cần categories
5. PackageDetailController.cs - Xóa nếu Package không cần details riêng
6. PaymentMethodController.cs - Xóa nếu payment methods hardcode
7. PromptInstanceDetailController.cs - Xóa nếu PromptInstance không cần details
8. RolesController.cs - Xóa nếu roles hardcode
9. UsersController.cs - Giữ nếu cần admin quản lý users
```

### ✅ GIỮ LẠI (13 Controllers):
```
1. AuthController.cs
2. WalletController.cs
3. PackageController.cs
4. OrderController.cs
5. TemplateArchitectureController.cs
6. PromptInstanceController.cs
7. AIController.cs
8. StorageTemplatesController.cs
9. PostController.cs
10. CartController.cs
11. PaymentsController.cs
12. ExpectedOutputController.cs
13. TransactionController.cs
```

